using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DEPRECADO. Primeira versao do AI do carro circa 2016/2017!
/// </summary>
public class AIController : MonoBehaviour
{
    NWH.VehiclePhysics.VehicleController car;

    Waypoint[] waypoints;
    int atual = 0;

    NWH.VehiclePhysics.VehicleController playerCarro;

    float newSteer;
    Vector3 steerVector;
    float anguloRaycast = 35f;

    public float distanciaVer;
    public float globalOffset;

    Vector3 posicaoIr;
    Vector3 ondeTasAir;

    float agressividade;
    float exatidao;

    public bool select;

    public LayerMask lm;
    public LayerMask lm2;

    bool virarDireita = false;
    bool virarEsquerda = false;

    float raycastDist = 5f;

    RaycastHit hit;

    float angleTime = 0f;
    float lastAngle = 999f;

    void Awake()
    {
        waypoints = GameObject.FindGameObjectWithTag("Waypoint_Layout0").GetComponentsInChildren<Waypoint>();
        car = GetComponent<NWH.VehiclePhysics.VehicleController>();
        agressividade = Random.Range(5f, 15f);
        exatidao = Random.Range(-5f, 5f);
    }

    void Start()
    {
        //playerCarro = GameObject.FindGameObjectWithTag("Player").GetComponent<NWH.VehiclePhysics.VehicleController>();
    }

    void FixedUpdate()
    {
        virarDireita = false;
        virarEsquerda = false;

        if (atual == 0)
        {
            posicaoIr = waypoints[atual].transform.position;
        }

        if (atual == waypoints.Length)
        {
            atual = 0;
        }

        if (car.SpeedKPH <= (waypoints[atual].velocidadeRecomendada - globalOffset) - agressividade)
        {
            /*if (car.volta != 0)
            {
                car.acc = 1f + (0.15f * (car.posicao - playerCarro.posicao));
            }
            else
            {
                car.acc = 1f;
            }*/

            car.input.Vertical = 1f;
        }
        /*else if (car.SpeedKPH <= waypoints[atual].velocidadeRecomendada)
        {
            car.input.Vertical = 0.15f;
        }*/
        else if (car.SpeedKPH >= (waypoints[atual].velocidadeRecomendada - globalOffset) - agressividade)
        {
            car.input.Vertical = -1f;
        }

        //abrandar se tiver fora da pista

        /*if (!car.Wheels[0].wheelController.activeFrictionPreset.name.Contains("Tarmac"))
        {
            if (car.SpeedKPH > 35f)
            {
                car.input.Vertical = -1f;
            }
        }*/

        Vector3 targetDir = waypoints[atual].transform.position - transform.position;

        if (Vector3.Angle(targetDir, transform.forward) > 45f)
        {
            if (car.SpeedKPH > 35f)
            {
                car.input.Vertical = -1f;
                if (select)
                {
                    Debug.Log("Travar 1");
                }
            }
        }       
        else if (Vector3.Angle(targetDir, transform.forward) > 30f)
        {
            if (car.SpeedKPH > 35f)
            {
                car.input.Vertical = -0.5f;
                if (select)
                {
                    Debug.Log("Travar 0.5");
                }
            }
        }
        else if (Vector3.Angle(targetDir, transform.forward) > 15f)
        {
            if (car.SpeedKPH > 35f)
            {
                car.input.Vertical = -0.2f;
                if (select)
                {
                    Debug.Log("Travar 0.2");
                }
            }
        }

        angleTime += Time.fixedDeltaTime;

        if (angleTime>0.3f)
        {
            angleTime = 0f;

            if (lastAngle==999f)
            {
                lastAngle = Vector3.Angle(targetDir, transform.forward);
                return;
            }

            float nowAngle = Vector3.Angle(targetDir, transform.forward);

            //Debug.Log("Novo: " + nowAngle + "- ant: " + lastAngle);

            if (car.SpeedKPH > 60f)
            {
                if (nowAngle+1.5 < lastAngle)
                {
                    car.input.Vertical = -1f;
                }
            }

            lastAngle = Vector3.Angle(targetDir, transform.forward);
        }

        /*
        steerVector = transform.InverseTransformPoint(new Vector3(waypoints[atual].transform.position.x, transform.position.y, waypoints[atual].transform.position.z));
        newSteer = Mathf.Clamp(((steerVector.x / steerVector.magnitude) * car.RodaGuiarGrafico.Evaluate(car.velocidadeKMH))/3f, -1f, 1f);

        car.gui = newSteer;

        if (steerVector.magnitude <= distanciaVer + (car.velocidadeKMH / 70f))
        {
            atual++;
        }
        */

        Vector3 objetivo = posicaoIr - transform.position;

        Vector3 anguloLocal = transform.InverseTransformPoint(posicaoIr);

        ondeTasAir = anguloLocal;

        float anguloRodas = Mathf.Atan2(anguloLocal.x, anguloLocal.z) * Mathf.Rad2Deg;

        anguloRodas = Mathf.Clamp(anguloRodas, -car.steering.highSpeedAngle, car.steering.highSpeedAngle);
        anguloRodas /= car.steering.highSpeedAngle;

        float sqrDist = (raycastDist * raycastDist);

        //DiagonalEsquerdo
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-Mathf.Cos(anguloRaycast * Mathf.Rad2Deg), 0f, Mathf.Sin(anguloRaycast * Mathf.Rad2Deg))), out hit, raycastDist, lm2))
        {
            car.input.Horizontal = (sqrDist / (transform.position - hit.point).sqrMagnitude) / sqrDist;
            //car.gui = 1.0f - ((transform.position - hit.point).sqrMagnitude) / (raycastDist * raycastDist);
            virarEsquerda = true;
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(Mathf.Cos(anguloRaycast * Mathf.Rad2Deg), 0f, Mathf.Sin(anguloRaycast * Mathf.Rad2Deg))), out hit, raycastDist, lm2))
        {
            car.input.Horizontal = -(sqrDist / (transform.position - hit.point).sqrMagnitude) / sqrDist;
            //car.gui = -(1.0f - ((transform.position - hit.point).sqrMagnitude) / (raycastDist * raycastDist));
            virarDireita = true;
        }
        else
        {
            car.input.Horizontal = anguloRodas;
        }

        car.input.Horizontal = Mathf.Clamp(car.input.Horizontal, -1f, 1f);

        if (select)
        {
            //Debug.Log(car.gui);
            //Debug.Log(car.acc);
            //Debug.Log(car.mudanca);

            //Debug.Log(waypoints[atual].velocidadeRecomendada+"-"+ (waypoints[atual].velocidadeRecomendada-agressividade).ToString());
        }

        if (Vector3.Distance(transform.position, posicaoIr) <= distanciaVer + (car.SpeedKPH / 70f))
        {
            lastAngle = 999f;
            atual++;
            if (atual >= waypoints.Length - 1)
            {
                return;
            }

            posicaoIr = waypoints[atual].transform.position;

            if (atual > 0 && atual < waypoints.Length - 1)
            {
                Vector3 posFuturo = waypoints[atual + 1].transform.position;
                Vector3 posAnterior = waypoints[atual - 1].transform.position;

                Vector3 direcao = posFuturo - posAnterior;

                Vector3 normal = Vector3.Cross(direcao, Vector3.up).normalized;


                posicaoIr += normal * Vector2.Dot(new Vector2(transform.forward.x, transform.forward.z), new Vector2(normal.x, normal.z)) * exatidao;

                if (Physics.Raycast(waypoints[atual].transform.position, posicaoIr - waypoints[atual].transform.position, out hit, lm.value))
                {
                    posicaoIr = hit.point;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, posicaoIr);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.TransformPoint(ondeTasAir));
        Gizmos.color = virarDireita ? Color.yellow : Color.blue;
        Gizmos.DrawLine(transform.position, transform.TransformPoint(new Vector3(Mathf.Cos(anguloRaycast * Mathf.Rad2Deg), 0f, Mathf.Sin(anguloRaycast * Mathf.Rad2Deg)) * raycastDist));
        Gizmos.color = virarEsquerda ? Color.green : Color.blue;
        Gizmos.DrawLine(transform.position, transform.TransformPoint(new Vector3(-Mathf.Cos(anguloRaycast * Mathf.Rad2Deg), 0f, Mathf.Sin(anguloRaycast * Mathf.Rad2Deg)) * raycastDist));
    }
}