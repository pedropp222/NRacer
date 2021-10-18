using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Terceira versao do AI de corridas, uma das melhores. Mas com varios problemas de curvaturas,
/// gradientes da pista, etc.
/// </summary>
public class AIControllerV3 : MonoBehaviour
{
    NWH.VehiclePhysics.VehicleController car;

    public dotCalculator waypointDot;

    public List<Waypoint> waypoints;
    public int atual = 0;

    public float distanciaVer;
    float orDist;

    bool foundBrakingZone;

    Vector3 lastPos;
    Vector3 nodePos;

    public AnimationCurve brakeVelocity;
    public AnimationCurve brakeVelocityAIVisual;

    public static AnimationCurve brakeVelocityAI;

    AnimationCurve useCurve;

    public bool useAILearn;

    public float steerSpeed = 10f;

    public float brakeAcceleration = 0f;

    Waypoint brakePoint;
    float lastDot = 99f;

    public float lastCurve = 1f;

    Carro_HUD volta;
    Carro_HUD playerCar;

    CarroVolta v;

    //variaveis debug;
    int p = 0;

    bool go = false;

    void Awake()
    {
        ///TODO
        ///Melhorar aqui isto da inicializaçao, quando a pista tiver varios layouts vai-se ter que organizar
        ///melhor isto!
        ///

        car = GetComponent<NWH.VehiclePhysics.VehicleController>();

        GameObject way = GameObject.FindGameObjectWithTag("Waypoint_Layout0");
        waypointDot = way.GetComponent<dotCalculator>();

        waypoints = new List<Waypoint>();

        for(int i = 0; i < way.transform.childCount;i++)
        {
            waypoints.Add(way.transform.GetChild(i).GetComponent<Waypoint>());
        }

        orDist = distanciaVer;

        car.transmission.transmissionType = NWH.VehiclePhysics.Transmission.TransmissionType.Manual;

        volta = GetComponent<Carro_HUD>();
        v = GetComponent<CarroVolta>();

        if(useAILearn)
        {
            useCurve = brakeVelocityAI;
        }
        else
        {
            useCurve = brakeVelocity;
        }
    }

    public void AtualizarAICurve()
    {
        useCurve = brakeVelocityAI;
        brakeVelocityAIVisual = useCurve;
    }

    private void FixedUpdate()
    {
        if (!go)
        {
            //nao pode andar, fica a acelerar feito maluco ate conseguir!
            car.input.Handbrake = 1f;
            car.transmission.Gear = 0;
            car.input.Vertical = 1f;
            return;
        }

        if (atual == waypoints.Count)
        {
            //loop
            atual = 0;
        }

        nodePos = waypoints[atual].transform.position;

        //Verificar proximos 5 nodes qual deles tem a curvatura mais acentuada e guardalo
        if (!foundBrakingZone)
        {
            int index = 0;
            float currentDot = 0f;

            //encontrar O MAIS acentuado dos 5
            for (int i = atual; i < atual+5; i++)
            {
                if (i==waypoints.Count)
                {
                    break;
                }

                currentDot = waypointDot.CalculateDot(i);

                if (currentDot<lastDot)
                {
                    lastDot = currentDot;
                    brakePoint = waypoints[i];
                    index = i;
                }
            }

            //agora verificar os proximos 2 nodes para verificar melhor o futuro da situaçao

            float dot1, dot2 = dot1 = 0.99f;

            index++;
            if (index != waypoints.Count)
            dot1 = waypointDot.CalculateDot(index);

            index++;
            if(index != waypoints.Count)
            dot2 = waypointDot.CalculateDot(index);

            float final = (lastDot + dot1 + dot2) / 3f;

            lastDot = final;

            //so marcar algo como curva se realmente tiver o minimo de curvatura (evitar falsos positivos)
            if (lastDot < 0.9905f || currentDot < 0.99f)
            {
                foundBrakingZone = true;
            }
        }

        car.input.Vertical = Rubberbanding();

        if (foundBrakingZone)
        {
            //calcular distancia a esse ponto
            float dist1 = Vector3.Distance(transform.position, brakePoint.transform.position);

            //se distancia for menor do que o recomendado
            if (dist1 <= BrakingDistance(lastDot))
            {
                p++;
                //if (p == 1) Debug.Log("TRAVAR AGORA");
                car.input.Vertical = -1f;
                if (car.SpeedKPH < GetFinalVelocity(lastDot))
                {
                    car.input.Vertical = 0f;
                }
            }
            //se esta muito proximo do node ou se ja o ultrapassou
            if (brakePoint != null)
            {
                if (dist1 <= 1f || dist1 > Vector3.Distance(lastPos, brakePoint.transform.position))
                {
                    foundBrakingZone = false;
                    p = 0;
                    brakePoint = null;
                    lastCurve = lastDot;
                    lastDot = 99f;
                    //Debug.Log("Acabou curva");
                }
            }
        }


        //VIRAR
        Vector3 sv = transform.InverseTransformPoint(new Vector3(waypoints[atual].transform.position.x, transform.position.y, waypoints[atual].transform.position.z));
        car.input.Horizontal = Mathf.Clamp((sv.x / sv.magnitude) * steerSpeed, -1f, 1f);


        //Alterar distancia de visao conforme velocidade
        if (car.SpeedKPH < 70f)
        {
            distanciaVer = 7f;
        }
        else
        {
            distanciaVer = orDist;
        }


        //Verificar se estamos proximos do node ou se o ja ultrapassamos, nestes casos, ir ja ver o proximo
        float distanciaNode = Vector3.Distance(transform.position, nodePos);

        /*if (distanciaNode <= distanciaVer + (car.SpeedKPH / 70f) || distanciaNode > Vector3.Distance(lastPos, waypoints[atual].transform.position))
        {
            atual++;
        }*/

        if (distanciaNode <= distanciaVer + (car.SpeedKPH / 70f))
        {
            atual++;
        }

        lastPos = transform.position;
    }

    /// <summary>
    /// Ir ao grafico buscar a velocidade recomendada + influencia da inclinaçao do terreno
    /// </summary>
    /// <param name="media">media de curvatura</param>
    /// <returns></returns>
    float GetFinalVelocity(float media)
    {
        float finalVelocity = useCurve.Evaluate(media);

        //BETA, velocidade final tem que ser menor em terreno inclinado claro
        if (transform.eulerAngles.x > 0.5f && transform.eulerAngles.x < 6f)
        {
            float reduce = transform.eulerAngles.x * (finalVelocity / 25f);

            if (reduce > 1f)
            {
                finalVelocity -= reduce;
                if (p == 0)
                {
                    //Debug.Log("Velocidade reduzida em " + reduce + "km/h-" + transform.eulerAngles.x);
                }
            }

        }

        return finalVelocity;
    }

    public float BrakingDistance(float media)
    {
        //breakingDistance = (finalVelocity*finalVelocity-initialVelocity*initialVelocity)/(2*deceleration*3.6*3.6)

        float finalVelocity = GetFinalVelocity(media);


        float finalDistance = (finalVelocity * finalVelocity - car.SpeedKPH * car.SpeedKPH) / (2f * brakeAcceleration * 3.6f * 3.6f);

        if (p==0)
        {
            //Debug.Log("Ponto curva " + brakePoint.name + " - distancia atual: " + Vector3.Distance(transform.position, brakePoint.transform.position) + " - recomendado: " + finalDistance);
            //Debug.Log("Velocidade final: " + finalVelocity);
        }

        return finalDistance;
    }

    /// <summary>
    /// Destravar e Lançar o carro, deixa-lo andar
    /// </summary>
    public void Launch()
    {
        go = true;
        atual = 0;
        car.input.Handbrake = 0f;
        car.transmission.Gear = 1;
        car.transmission.transmissionType = NWH.VehiclePhysics.Transmission.TransmissionType.AutomaticSequential;
    }


    //Implementaçao basica de rubberbanding, a partir da 2º volta começa a controlar a
    //aceleraçao maxima em relaçao a sua posiçao relativa a do player
    public float Rubberbanding()
    {
        if (v.voltas==0)
        {
            return 1f;
        }

        if (playerCar==null)
        {
            playerCar = GameObject.FindGameObjectWithTag("Vehicle").GetComponent<Carro_HUD>();
        }

        return Mathf.Clamp(1f+((volta.pos-playerCar.pos)*0.15f),0.5f,1f);
    }
}