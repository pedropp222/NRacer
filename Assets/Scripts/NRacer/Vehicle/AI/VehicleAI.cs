using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.VehiclePhysics;
using System;
using JetBrains.Annotations;
using Assets.Scripts.NRacer.Controllers;

/// <summary>
/// Versao FINAL E VERDADEIRA (so demorou 5 anos) do AI do carro. Criaçao automatica de um AI super
/// competente independentemente do cenario (terra ou asfalto), sem trabalho NENHUM!
/// </summary>
public class VehicleAI : MonoBehaviour
{
    VehicleController vehicle;
    Rigidbody rBody;

    List<Transform> wayps;
    int waypAtual = -1;

    public float brakeDeceleration;

    Vector3 currentPosWaypoints;
    Vector3 targetPos;
    Vector3 brakeLocation;

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

    public bool useRubberbanding = true;

    public bool limitInputSpeed;
    public float maxInput;

    private float aiDesempenho;
    private float playerDesempenho;

    bool playerMaisFraco;

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

        TrackManager tm = FindAnyObjectByType<TrackManager>();

        GameObject go = tm.GetAiWaypoints();

        for (int i = 0; i < go.transform.childCount; i++)
        {
            wayps.Add(go.transform.GetChild(i));
        }

        aiDesempenho = GetComponent<CarroStats>().GetPontosDesempenho();

        if (tm.playerCarro != null)
        {
            playerCar = tm.playerCarro.GetComponent<Carro_HUD>();
            playerDesempenho = playerCar.GetComponent<CarroStats>().GetPontosDesempenho();
            if (playerDesempenho <= aiDesempenho)
            {
                Debug.Log("Desempenho e mais fraco do que este veiculo, rubberbanding mais intenso");
                playerMaisFraco = true;
            }
        }


    }

    private void FixedUpdate()
    {
        if (vehicle == null)
        {
            Debug.LogWarning("Veiculo nulo!");
            return;
        }

        if (backingUp) return;

        if (!andar)
        {
            //nao pode andar, fica a acelerar feito maluco ate conseguir!
            if (vehicle.transmission.transmissionType != Transmission.TransmissionType.Manual)
            {
                vehicle.transmission.transmissionType = NWH.VehiclePhysics.Transmission.TransmissionType.Manual;
            }
            vehicle.input.Handbrake = 1f;
            vehicle.transmission.Gear = 0;
            vehicle.input.Vertical = 0.5f;
            return;
        }

        int waypAtual;

        currentPosWaypoints = EncontrarPosicao(transform.position, out waypAtual, true);

        targetPos = OffsetPosicao(currentPosWaypoints, 15f + (vehicle.SpeedKPH / 20f));

        brakeLocation = OffsetPosicao(targetPos, BrakingDistance(110f));

        //aceleraçao base
        vehicle.input.Vertical = 1f;


        //VIRAR
        Vector3 sv = transform.InverseTransformPoint(new Vector3(targetPos.x, transform.position.y, targetPos.z));
        vehicle.input.Horizontal = Mathf.Clamp((sv.x / sv.magnitude) * 3.5f, -1f, 1f);


        //maximo que pode virar a velocidade atual
        float maxSteerAngle = Mathf.Abs(Mathf.Lerp(vehicle.steering.lowSpeedAngle, vehicle.steering.highSpeedAngle, vehicle.Speed / vehicle.steering.crossoverSpeed));


        //distanciaAB * cos(AB)
        //float curvaturaDot = Mathf.Min(Vector3.Dot(transform.forward.normalized, (brakeLocation-targetPos).normalized), Vector3.Dot(transform.forward.normalized, (targetPos - transform.position).normalized));
        float curvaturaDot = Mathf.Min(Vector3.Dot(rBody.velocity.normalized, (brakeLocation - targetPos).normalized), Vector3.Dot(rBody.velocity.normalized, (targetPos - transform.position).normalized));

        //Debug.Log(curvaturaDot);

        float futureCurvaturaDot = PreverPosicaoFuturo();


        float cosAngle = Mathf.Cos(maxSteerAngle * Mathf.Deg2Rad);


        float medianCurvatura = (curvaturaDot + futureCurvaturaDot) / 2f;

        //brake
        float B = (cosAngle - medianCurvatura) / (cosAngle - (brakeAgressive));

        if (useNewCalc)
        {
            B = (cosAngle - curvaturaDot) / (cosAngle - (brakeAgressive)) * (BrakingDistance(60f) / Vector3.Distance(transform.position, brakeLocation)) - (1f-vehicle.axles[1].rightWheel.SideSlipPercent)/3f;
        }

        B = Mathf.Clamp01(B);
        B = BlookUp.Evaluate(B);

        //acelerar
        float A = (medianCurvatura - cosAngle) / (1f - cosAngle);

        if (useNewCalc)
        {
            A = (curvaturaDot - cosAngle) / (1f - cosAngle) + (1f - vehicle.input.Horizontal) + 1.25f - ((vehicle.axles[0].leftWheel.SideSlipPercent + vehicle.axles[0].rightWheel.SideSlipPercent) / 2f)/1.4f;
        }

        A = Mathf.Clamp01(A) * dificuldade;
        A = Mathf.Clamp01(A);
        A = AlookUp.Evaluate(A);

        if (useRubberbanding)
        {
            Rubberbanding();
        }

        if (vehicle.SpeedKPH < 3f && Physics.Raycast(transform.position, transform.forward, 5f))
        {
            StartCoroutine(BackUp());
        }
        else
        {
            if (vehicle.SpeedKPH > 20f)
            {
                //final input
                if (vehicle.SpeedKPH < 60f && limitInputSpeed)
                {
                    vehicle.input.Vertical = (A - B) * maxInput;
                }
                else
                {
                    /*if (useRubberbanding)
                    {
                        vehicle.input.Vertical = (A - B) * Rubberbanding();
                    }
                    else
                    {
                        vehicle.input.Vertical = (A - B);
                    }*/
                    vehicle.input.Vertical = (A - B);
                }
            }
        }
    }

    bool backingUp = false;

    private IEnumerator BackUp()
    {
        Debug.Log("Run Back up");
        if (!backingUp)
        {
            vehicle.input.Horizontal *= -1f;
            vehicle.transmission.transmissionType = Transmission.TransmissionType.Manual;
            backingUp = true;
            vehicle.transmission.Gear = -1;
            vehicle.input.Vertical = 1;
            yield return new WaitForSeconds(1.5f);
            vehicle.transmission.Gear = 1;
            backingUp = false;
            vehicle.transmission.transmissionType = Transmission.TransmissionType.AutomaticSequential;
        }
    }

    public void Rubberbanding()
    {


        /*if (v.voltas == 0)
        {
            return 1f;
        }*/

        if (playerCar == null)
        {
            return;
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
                    currentQuant += 0.0006f;
                }
                else
                {
                    currentQuant -= 0.0009f;
                }

                currentQuant = Mathf.Clamp(currentQuant, v.UltimaVolta() && playerMaisFraco ? -0.4f : -0.24f, v.UltimaVolta()?0f:0.2f);
                quant *= 1 + currentQuant;

                if (v.UltimaVolta() && quant >= 0.95f)
                {
                    quant = 0.95f - (playerMaisFraco ? 0.12f : 0f);
                }

                vehicle.engine.maxPower = initialPower * quant;

                /*if (quant > 1f)
                {
                    //se este carro tiver atras do player, aumentar o HP do carro
                    vehicle.engine.maxPower = initialPower * quant;
                }
                else
                {
                    vehicle.engine.maxPower = initialPower * quant;
                }*/
            }

            //Debug.Log("Rubberbanding multiplier: " + quant+" --- "+currentQuant);
            dbugI = 0;
        }
    }

    private float PreverPosicaoFuturo()
    {
        Vector3 posFuturo = transform.position + rBody.velocity * 0.2f;
        Vector3 futureForward = transform.forward + rBody.velocity * 0.2f;

        int B = 0;

        Vector3 fPos = EncontrarPosicao(posFuturo, out B);

        Vector3 futureTargetPos = OffsetPosicao(fPos, 15f + (vehicle.SpeedKPH / 20f));

        Vector3 futureBrakeLocation = OffsetPosicao(futureTargetPos, BrakingDistance(60f));

        float futureCurvaturaDot = Mathf.Min(Vector3.Dot(futureForward.normalized, (futureBrakeLocation - futureTargetPos).normalized), Vector3.Dot(futureForward.normalized, (futureTargetPos - posFuturo).normalized));

        return futureCurvaturaDot;
    }

    public float BrakingDistance(float media)
    {
        float finalDistance = (media * media - vehicle.SpeedKPH * vehicle.SpeedKPH) / (2f * brakeDeceleration * 3.6f * 3.6f);

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

    private int TryWrap(int value, int increase, int max)
    {
        if (value + increase > max)
        {
            value = 0;
        }
        else
        {
            value = value + increase;
        }
        return value;
    }

    public Vector3 EncontrarPosicao(Vector3 worldPos, out int escolhidoB, bool waypoint = false)
    {
        Vector3 result = Vector3.zero;

        float startDistance = 999999f;

        int escolhidoA = -1;
        escolhidoB = -1;
        int escolhidoA2 = -1;

        if (waypoint)
        {
            int start = waypAtual == -1 ? 0 : waypAtual;
            int end = waypAtual == -1 ? wayps.Count : TryWrap(waypAtual, 2, wayps.Count - 1);

            if (start > end)
            {
                start = end;
                end++;
            }

            for (int i = start; i < end; i++)
            {
                float dist = Vector3.Distance(worldPos, wayps[i].position);

                if (dist < startDistance)
                {
                    startDistance = dist;
                    escolhidoA = i;
                    waypAtual = i;
                }
            }
        }
        else
        {
            for (int i = waypAtual; i < Mathf.Min(waypAtual+7, wayps.Count); i++)
            {
                float dist = Vector3.Distance(worldPos, wayps[i].position);

                if (dist < startDistance)
                {
                    startDistance = dist;
                    escolhidoA = i;
                }
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
        Debug.Log("O carro AI foi lancado");
        andar = true;
        vehicle = GetComponent<VehicleController>();
        vehicle.input.Handbrake = 0f;
        vehicle.transmission.Gear = !andamento ? 1 : vehicle.transmission.Gear;
        vehicle.transmission.transmissionType = NWH.VehiclePhysics.Transmission.TransmissionType.AutomaticSequential;
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, currentPosWaypoints);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, brakeLocation);

        if (waypAtual == -1) return;
        Gizmos.DrawCube(wayps[waypAtual].position, Vector3.one);
    }*/
}