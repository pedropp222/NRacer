using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.VehiclePhysics;

/// <summary>
/// DEPRECADO. tentava criar um ai competente automaticamente ao ajustar os valores de velocidade
/// dependendo da curvatura que encontrava e se conseguia fazer ou nao
/// </summary>
public class AI_Learner : MonoBehaviour
{
    AIControllerV3 ai;

    VehicleController vehicle;

    Vector3 startLocation, startRotation;
    readonly float maxTime = 2f;
    float curTime = 0f;
    int numberWheels = 0;

    public bool masterCar;

    private void Awake()
    {
        if (masterCar&&AIControllerV3.brakeVelocityAI == null)
        {
            AIControllerV3.brakeVelocityAI = new AnimationCurve();
            //X, intensidade, Y velocidade km/h
            AIControllerV3.brakeVelocityAI.AddKey(0f, 200f);
            AIControllerV3.brakeVelocityAI.AddKey(1f, 200f);
        }
    }

    private void Start()
    {
        vehicle = GetComponent<VehicleController>();
        ai = GetComponent<AIControllerV3>();

        startLocation = transform.position;
        startRotation = transform.eulerAngles;     

        ai.AtualizarAICurve();
    }

    private void FixedUpdate()
    {
        numberWheels = 0;

        //RODAS QUE ESTAO FORA DA PISTA
        for(int i = 0; i < vehicle.Wheels.Count; i++)
        {
            if (vehicle.Wheels[i].CurrentGroundEntity != null)
            {
                if (vehicle.Wheels[i].CurrentGroundEntity.name == "Gravel")
                {
                    numberWheels++;
                }
            }
        }

        if (numberWheels > 0)
        {
            curTime += Time.fixedDeltaTime*numberWheels;
            if (curTime >= maxTime)
            {
                //ACABOU, O CARRO ESTA A DEMASIADO TEMPO FORA DA PISTA
                Reset();
            }
        }
        else
        {
            curTime = 0f;
        }
    }

    private void Update()
    {
        //se clicar no enter, fazer debug da tabela atual (como e static nao aparece no unity)
        //apenas 1 dos carros ser master para nao spamar a consola
        if (masterCar)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                for(int i=0; i < AIControllerV3.brakeVelocityAI.length;i++)
                {
                    Debug.Log(AIControllerV3.brakeVelocityAI.keys[i].time+" - "+ AIControllerV3.brakeVelocityAI.keys[i].value);
                }
            }
        }
    }

    private void Reset()
    {
        //(a magia) Criaçao automatica da curva de velocidades
        float curva = Mathf.Round(ai.lastCurve * 1000.0f) * 0.001f;

        bool existsKey = false;

        for(int i = 0; i < AIControllerV3.brakeVelocityAI.length;i++)
        {
            if (AIControllerV3.brakeVelocityAI.keys[i].time==curva)
            {
                //reduzir em 5km/h da proxima vez,ok?
                existsKey = true;
                float speed = AIControllerV3.brakeVelocityAI.keys[i].value;
                if(speed > 100f)
                {
                    speed -= 10f;
                }
                else
                {
                    speed -= 5f;
                }

                AIControllerV3.brakeVelocityAI.MoveKey(i, new Keyframe(curva,speed));
                if (masterCar) Debug.Log("Fez reset, proxima curva " + curva + " a " + AIControllerV3.brakeVelocityAI.keys[i].value + "km/h");
                break;
            }
        }

        if (!existsKey)
        {
            int k = AIControllerV3.brakeVelocityAI.AddKey(curva, 150f);

            AIControllerV3.brakeVelocityAI.MoveKey(k, new Keyframe(curva, (AIControllerV3.brakeVelocityAI.keys[k-1].value+ AIControllerV3.brakeVelocityAI.keys[k + 1].value)/2f));

            Debug.Log("Fez reset, proxima curva " + curva + " a " + AIControllerV3.brakeVelocityAI.keys[k].value + "km/h");
        }

        SetCurveLinear(AIControllerV3.brakeVelocityAI);

        ai.AtualizarAICurve();
        //começar de novo com as novas ideias em mente
        transform.position = startLocation;
        transform.eulerAngles = startRotation;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        curTime = 0f;
        ai.Launch();
    }
    
    //Pegar no grafico atual e meter os waypoints em modo linear
    public static void SetCurveLinear(AnimationCurve curve)
    {
        for (int i = 0; i < curve.keys.Length; ++i)
        {
            float intangent = 0;
            float outtangent = 0;
            bool intangent_set = false;
            bool outtangent_set = false;
            Vector2 point1;
            Vector2 point2;
            Vector2 deltapoint;
            Keyframe key = curve[i];

            if (i == 0)
            {
                intangent = 0; intangent_set = true;
            }

            if (i == curve.keys.Length - 1)
            {
                outtangent = 0; outtangent_set = true;
            }

            if (!intangent_set)
            {
                point1.x = curve.keys[i - 1].time;
                point1.y = curve.keys[i - 1].value;
                point2.x = curve.keys[i].time;
                point2.y = curve.keys[i].value;

                deltapoint = point2 - point1;

                intangent = deltapoint.y / deltapoint.x;
            }
            if (!outtangent_set)
            {
                point1.x = curve.keys[i].time;
                point1.y = curve.keys[i].value;
                point2.x = curve.keys[i + 1].time;
                point2.y = curve.keys[i + 1].value;

                deltapoint = point2 - point1;

                outtangent = deltapoint.y / deltapoint.x;
            }

            key.inTangent = intangent;
            key.outTangent = outtangent;
            curve.MoveKey(i, key);
        }
    }
}
