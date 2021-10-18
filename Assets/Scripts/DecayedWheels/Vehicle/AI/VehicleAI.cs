using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.VehiclePhysics;
using System;

/// <summary>
/// Versao FINAL E VERDADEIRA (so demorou 5 anos) do AI do carro. Criaçao automatica de um AI super
/// competente independentemente do cenario (terra ou asfalto), sem trabalho NENHUM!
/// </summary>
public class VehicleAI : MonoBehaviour
{
    VehicleController vehicle;
    Rigidbody rBody;

    List<Transform> wayps;

    public float brakeDeceleration;

    public GameObject debugObj;
    public GameObject debugObj2;
    public GameObject debugObj3;

    Vector3 currentPosWaypoints;
    Vector3 targetPos;

    public float brakeAgressive = -1f;

    public bool useNewCalc;
    public float dificuldade;

    public AnimationCurve AlookUp;
    public AnimationCurve BlookUp;

    public float step;

    bool andar = false;

    Carro_HUD volta;
    CarroVolta v;
    Carro_HUD playerCar;

    int dbugI = 0;

    float initialPower;

    float currentQuant = 0f;

    private void Start()
    {
        //Provisorio: vai buscar a lista de waypoints do layout 1 por defeito
        //no futuro tem que ser fornecido o layout a partir do corridaSettings ou assim

        vehicle = GetComponent<VehicleController>();
        rBody = GetComponent<Rigidbody>();
        volta = GetComponent<Carro_HUD>();
        v = GetComponent<CarroVolta>();
        wayps = new List<Transform>();

        initialPower = vehicle.engine.maxPower;

        GameObject go = GameObject.FindGameObjectWithTag("Waypoint_Layout0");

        for (int i = 0; i < go.transform.childCount; i++)
        {
            wayps.Add(go.transform.GetChild(i));
        }

        vehicle.transmission.transmissionType = NWH.VehiclePhysics.Transmission.TransmissionType.Manual;
        playerCar = GameObject.FindGameObjectWithTag("Vehicle")?.GetComponent<Carro_HUD>();
    }

    private void FixedUpdate()
    {
        if (vehicle==null)
        {
            Debug.LogWarning("Veiculo nulo!");
            return;
        }

        if (!andar)
        {
            //nao pode andar, fica a acelerar feito maluco ate conseguir!
            vehicle.input.Handbrake = 1f;
            vehicle.transmission.Gear = 0;
            vehicle.input.Vertical = 0.5f;
            return;
        }

        int waypAtual;

        currentPosWaypoints = EncontrarPosicao(transform.position, out waypAtual);

        targetPos = OffsetPosicao(currentPosWaypoints, 15f + (vehicle.SpeedKPH / 20f));

        Vector3 brakeLocation = OffsetPosicao(targetPos, BrakingDistance(0f));

        if (debugObj != null)
        {
            debugObj.transform.position = targetPos;
            debugObj2.transform.position = currentPosWaypoints;
            debugObj3.transform.position = brakeLocation;
        }

        //aceleraçao base
        vehicle.input.Vertical = 1f;

        

        //VIRAR
        Vector3 sv = transform.InverseTransformPoint(new Vector3(targetPos.x, transform.position.y, targetPos.z));
        vehicle.input.Horizontal = Mathf.Clamp((sv.x / sv.magnitude) * 3.5f, -1f, 1f);

        //angulo entre targetVirar e nos
        float lateralAngle = Vector3.Angle(targetPos - transform.position, transform.forward);

        //maximo que pode virar a velocidade atual
        float maxSteerAngle = Mathf.Abs(Mathf.Lerp(vehicle.steering.lowSpeedAngle, vehicle.steering.highSpeedAngle, vehicle.Speed / vehicle.steering.crossoverSpeed));

        float lateralAngleBrake = Vector3.Angle(brakeLocation - transform.position, transform.forward);


        float curvaturaTeuSteer = Mathf.Cos(maxSteerAngle);


        //distanciaAB * cos(AB)
        //float curvaturaDot = Mathf.Min(Vector3.Dot(transform.forward.normalized, (brakeLocation-targetPos).normalized), Vector3.Dot(transform.forward.normalized, (targetPos - transform.position).normalized));
        float curvaturaDot = Mathf.Min(Vector3.Dot(rBody.velocity.normalized, (brakeLocation-targetPos).normalized), Vector3.Dot(rBody.velocity.normalized, (targetPos - transform.position).normalized));

        //Debug.Log(curvaturaDot);

        float futureCurvaturaDot = PreverPosicaoFuturo();


        float cosAngle = Mathf.Cos(maxSteerAngle * Mathf.Deg2Rad);


        float medianCurvatura = (curvaturaDot + futureCurvaturaDot) / 2f;

        //brake
        float B = (cosAngle - medianCurvatura) / (cosAngle - (brakeAgressive));

        if (useNewCalc)
        {
            B = (cosAngle - curvaturaDot) / (cosAngle - (brakeAgressive)) * (BrakingDistance(0f) / Vector3.Distance(transform.position, brakeLocation));
        }

        B = Mathf.Clamp01(B);
        B = BlookUp.Evaluate(B);

        //acelerar
        float A = (medianCurvatura - cosAngle) / (1f - cosAngle);

        if (useNewCalc)
        {
            A = (curvaturaDot - cosAngle) / (1f - cosAngle) + (1f - vehicle.input.Horizontal) + 1f - ((vehicle.axles[0].leftWheel.SideSlipPercent + vehicle.axles[0].rightWheel.SideSlipPercent) / 2f);
        }

        A = Mathf.Clamp01(A) * dificuldade;
        A = Mathf.Clamp01(A);
        A = AlookUp.Evaluate(A);


        if (vehicle.SpeedKPH > 20f)
        {
            //final input
            vehicle.input.Vertical = (A - B)*Rubberbanding();
        }
    }

    public float Rubberbanding()
    {


        /*if (v.voltas == 0)
        {
            return 1f;
        }*/

        if (playerCar == null)
        {
            return 1f;           
        }

        //quanto maior a diferença entre este e o player, menos acelera
        //ai - 2º lugar | player - 4º lugar = maximo de 0.9f aceleraçao

        float quant = 1f + ((volta.pos - playerCar.pos) * 0.05f);

        dbugI++;
        if (dbugI == 10)
        {
            if (playerCar.pos != -1)
            {
                if (quant >= 1.0f)
                {
                    currentQuant += 0.0015f;
                }
                else
                {
                    currentQuant -= 0.0006f;
                }

                currentQuant = Mathf.Clamp(currentQuant, -0.3f, 0.2f);
                quant *= 1 + currentQuant;

                if (quant > 1f)
                {
                    //se este carro tiver atras do player, aumentar o HP do carro
                    vehicle.engine.maxPower = initialPower * quant;
                }
                else
                {
                    vehicle.engine.maxPower = initialPower;
                }
            }

            //Debug.Log("Rubberbanding multiplier: " + quant+" . "+currentQuant);
            dbugI = 0;
        }

        return Mathf.Clamp(quant, 0.5f, 1f);
    }

    private float PreverPosicaoFuturo()
    {
        Vector3 posFuturo = transform.position + rBody.velocity * 0.2f;
        Vector3 futureForward = transform.forward + rBody.velocity * 0.2f;

        int B = 0;

        Vector3 fPos = EncontrarPosicao(posFuturo, out B);

        Vector3 futureTargetPos = OffsetPosicao(fPos, 15f + (vehicle.SpeedKPH / 20f));

        Vector3 futureBrakeLocation = OffsetPosicao(futureTargetPos, BrakingDistance(0f));

        float futureCurvaturaDot = Mathf.Min(Vector3.Dot(futureForward.normalized, (futureBrakeLocation - futureTargetPos).normalized), Vector3.Dot(futureForward.normalized, (futureTargetPos - posFuturo).normalized));

        return futureCurvaturaDot;
    }

    public float BrakingDistance(float media)
    {
        float finalDistance = (50f * 50f - vehicle.SpeedKPH * vehicle.SpeedKPH) / (2f * brakeDeceleration * 3.6f * 3.6f);

        finalDistance = Mathf.Clamp(finalDistance, 0f, 300f);

        return finalDistance;
    }


    public Vector3 OffsetPosicao(Vector3 pos, float metros)
    {
        Vector3 result = Vector3.zero;

        int startT = -1;

        result = EncontrarPosicao(pos, out startT);

        for (int i = startT; i < wayps.Count; i++)
        {
            if (i > wayps.Count - 3)
            {
                result = wayps[0].position;
                return result;
            }

            for (float t = 0f; t <= 1f; t += step)
            {
                Vector3 f = Vector3.Lerp(wayps[i].position, wayps[i + 1].position, t);

                result = f;

                if (Vector3.Distance(pos, result) >= metros)
                {
                    return result;
                }
            }
        }

        return result;
    }

    public Vector3 EncontrarPosicao(Vector3 worldPos, out int escolhidoB)
    {
        Vector3 result = Vector3.zero;

        float startDistance = 999999f;

        int escolhidoA = -1;
        escolhidoB = -1;
        int escolhidoA2 = -1;

        for (int i = 0; i < wayps.Count; i++)
        {
            float dist = Vector3.Distance(worldPos, wayps[i].position);

            if (dist < startDistance)
            {
                startDistance = dist;
                escolhidoA = i;
            }
        }

        escolhidoB = escolhidoA + 1;

        if (escolhidoB >= wayps.Count)
        {
            escolhidoB = 0;
        }

        escolhidoA2 = escolhidoA - 1;

        if (escolhidoA2 < 0)
        {
            escolhidoA2 = wayps.Count - 1;
        }

        float minDistance = Mathf.Infinity;

        for (float t = 0f; t <= 1f; t += step)
        {
            Vector3 f = Vector3.Lerp(wayps[escolhidoA2].position, wayps[escolhidoB].position, t);

            float dist = (worldPos - f).sqrMagnitude;

            if (dist < minDistance)
            {
                minDistance = dist;
                result = f;
            }
        }

        return result;
    }

    /// <summary>
    /// Destravar e Lançar o carro
    /// </summary>
    public void LancarCarro(bool andamento = false)
    {
        andar = true;
        vehicle = GetComponent<VehicleController>();
        vehicle.input.Handbrake = 0f;
        vehicle.transmission.Gear = !andamento?1: vehicle.transmission.Gear;
        vehicle.transmission.transmissionType = NWH.VehiclePhysics.Transmission.TransmissionType.AutomaticSequential;
    }
}
