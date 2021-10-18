using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DEPRECADO: Segunda versao do AI de corridas, bastante melhor que o primeiro mas com muitas
/// falhas ainda.
/// </summary>
public class AIControllerV2 : MonoBehaviour
{
    NWH.VehiclePhysics.VehicleController car;

    dotCalculator waypointDot;

    Waypoint[] waypoints;
    int atual = 0;

    float newSteer;
    Vector3 steerVector;
    float anguloRaycast = 35f;

    public float distanciaVer;


    bool foundBrakingZone;
    Vector3 brakingPosition;

    float orDist;

    public AnimationCurve dotOffset;

    public AnimationCurve brakeVelocity;

    Vector3 posicaoIr;
    Vector3 ondeTasAir;

    float agressividade;
    float exatidao;

    public float steerSpeed = 10f;

    public float brakeAcceleration = 0f;

    int mn = 0;
    int p = 0;
    float savedMedia;

    Vector3 lastPos;
    void Awake()
    {
        GameObject way = GameObject.FindGameObjectWithTag("Waypoint_Layout0");
        waypointDot = way.GetComponent<dotCalculator>();
        waypoints = way.GetComponentsInChildren<Waypoint>();

        car = GetComponent<NWH.VehiclePhysics.VehicleController>();

        agressividade = Random.Range(5f, 15f);
        exatidao = Random.Range(-5f, 5f);

        orDist = distanciaVer;

        brakeVelocity = new AnimationCurve((Keyframe[])dotOffset.keys.Clone());

        for(int i = 0; i < brakeVelocity.keys.Length; i++)
        {
            Keyframe x = new Keyframe
            {
                time = dotOffset.keys[i].value,
                value = dotOffset.keys[i].time
            };

            brakeVelocity.MoveKey(i, x);
        }
    }

    private void Start()
    {
        car.input.Handbrake = 0f;
    }

    void FixedUpdate()
    {
        if (atual == 0)
        {
            posicaoIr = waypoints[atual].transform.position;
        }

        if (atual == waypoints.Length)
        {
            atual = 0;
        }

        /*if (car.SpeedKPH <= (waypoints[atual].velocidadeRecomendada - globalOffset) - agressividade)
        {
            car.input.Vertical = 1f;
        }
        else if (car.SpeedKPH >= (waypoints[atual].velocidadeRecomendada - globalOffset) - agressividade)
        {
            car.input.Vertical = -1f;
        }*/

        float offset = dotOffset.Evaluate(car.SpeedKPH);

        float media = 100f;

        if (atual + 2 >= waypoints.Length)
        {
            media = (waypointDot.CalculateDot(atual) + waypointDot.CalculateDot(0) + waypointDot.CalculateDot(1)) / 3f;
        }
        else if (atual + 1 >= waypoints.Length)
        {
            media = (waypointDot.CalculateDot(atual) + waypointDot.CalculateDot(0) + waypointDot.CalculateDot(1)) / 3f;
        }
        else
        {
            media = (waypointDot.CalculateDot(atual) + waypointDot.CalculateDot(atual + 1) + waypointDot.CalculateDot(atual + 2)) / 3f;
        }

        //Debug.Log("Media: " + media + " - "+ offset);

        if (media > offset)
        {           
            if (!foundBrakingZone)
            {
                car.input.Vertical = 1f;

            }
        }
        else
        {
            if (car.SpeedKPH > 50f)
            {
                if (!foundBrakingZone)
                {
                    foundBrakingZone = true;
                    brakingPosition = waypoints[atual + 3].transform.position;
                    savedMedia = media;
                }
                if (brakeAcceleration == 0f)
                {
                    car.input.Vertical = -1f;
                }
            }
            else
            {
                car.input.Vertical = 1f;
            }
        }

        if (foundBrakingZone)
        {
            mn++;
            float dist1 = Vector3.Distance(transform.position, brakingPosition);

            if (dist1 <= BrakingDistance(savedMedia))
            {
                p++;
                if (p==1)Debug.Log("TRAVAR AGORA");
                car.input.Vertical = -1f;
                if (car.SpeedKPH < GetFinalVelocity(savedMedia))
                {
                    car.input.Vertical = 0f;
                }
            }
            if (dist1 <= 5f || dist1 > Vector3.Distance(lastPos,brakingPosition))
            {
                foundBrakingZone = false;
                brakingPosition = Vector3.zero;
                mn = 0;
                p = 0;
                Debug.Log("Acabou curva");
            }
        }

        Vector3 sv = transform.InverseTransformPoint(new Vector3(waypoints[atual].transform.position.x, transform.position.y, waypoints[atual].transform.position.z));
        car.input.Horizontal = Mathf.Clamp((sv.x / sv.magnitude) * steerSpeed, -1f, 1f);

        if (car.SpeedKPH < 70f)
        {
            distanciaVer = 7f;
        }
        else
        {
            distanciaVer = orDist;
        }


        if (Vector3.Distance(transform.position, posicaoIr) <= distanciaVer + (car.SpeedKPH / 70f))
        {
            atual++;
            if (atual >= waypoints.Length - 1)
            {
                return;
            }

            posicaoIr = waypoints[atual].transform.position;

            /*if (atual > 0 && atual < waypoints.Length - 1)
            {
                Vector3 posFuturo = waypoints[atual + 1].transform.position;
                Vector3 posAnterior = waypoints[atual - 1].transform.position;

                Vector3 direcao = posFuturo - posAnterior;

                Vector3 normal = Vector3.Cross(direcao, Vector3.up).normalized;

                posicaoIr += normal * Vector2.Dot(new Vector2(transform.forward.x, transform.forward.z), new Vector2(normal.x, normal.z)) * exatidao;
            }*/

            lastPos = transform.position;
        }
    }

    float GetFinalVelocity(float media)
    {
        return brakeVelocity.Evaluate(media);
    }

    public float BrakingDistance(float media)
    {
        //breakingDistance = (finalVelocity*finalVelocity-initialVelocity*initialVelocity)/(2*deceleration*3.6*3.6)

        float finalVelocity = GetFinalVelocity(media);


        float finalDistance = (finalVelocity * finalVelocity - car.SpeedKPH * car.SpeedKPH) / (2f * brakeAcceleration * 3.6f*3.6f);

        if (finalDistance < 12f + (car.SpeedKPH / 30f))
        {
            finalDistance = 12f + (car.SpeedKPH / 30f);
        }

        if (mn==1)
        {
            Debug.Log("Encontrou braking zone."+ brakingPosition.ToString() +"--distancia:"+Vector3.Distance(transform.position,brakingPosition)+" - recomendado: "+finalDistance);
            Debug.Log("velocidade inicial: "+car.SpeedKPH+"--velocidade final: " + finalVelocity);
        }

        return finalDistance;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, posicaoIr);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.TransformPoint(ondeTasAir));
    }
}