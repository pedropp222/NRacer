using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.VehiclePhysics;
using BezierSolution;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class AISplineTest : MonoBehaviour
{
    VehicleController vehicle;

    BezierSpline spline;

    float maxSpeed = 10f;

    Rigidbody rigidbody;

    public Vector3 target;

    float location = 0f;

    float splineLength = 0f;

    public GameObject debugObj;
    public GameObject debugObj2;

    bool debug = true;
    public Text debugText;

    public float brakeDeceleration;

    float carPosition = -1f;
    float targetPosition = 0f;

    float maxSteerAngle = 0f;
    public Vector3 brakeLocation;
    float brakeLocationT = 0f;

    public float brakeAgressive = -1f;

    public AnimationCurve AlookUp;
    public AnimationCurve BlookUp;

    public bool useNewCalc = false;

    public float dificuldade = 1f;

    float lastT = -1f;

    private void Start()
    {
        vehicle = GetComponent<VehicleController>();

        spline = FindObjectOfType<BezierSpline>();

        rigidbody = GetComponent<Rigidbody>();

        splineLength = spline.GetLengthApproximately(0f,1f,100f);
        Debug.Log("Initializado spline com " + splineLength + "m");

        if (debugText == null)
        {
            debug = false;
        }
    }

    public float BrakingDistance(float media)
    {
        float finalDistance = (50f * 50f - vehicle.SpeedKPH * vehicle.SpeedKPH) / (2f * brakeDeceleration * 3.6f * 3.6f);

        finalDistance = Mathf.Clamp(finalDistance, 0f, 100f);

        return finalDistance;
    }

    public float GetOffsetTCustom(float start, float offset)
    {
        return start + (offset / splineLength);
    }

    public float GetOffsetT(bool car, float m)
    {
        if (car)
        {
            return carPosition + ((m + (vehicle.SpeedKPH / 30f)) / splineLength);
        }
        else
        {
            return targetPosition + ((m + (vehicle.SpeedKPH / 30f)) / splineLength);
        }
    }

    public Vector3 GetSplinePoint(float t)
    {
        return spline.GetPoint(t);
    }

    public float PreverPosicaoFuturo()
    {
        Vector3 posFuturo = transform.position + rigidbody.velocity * 0.2f;
        Vector3 futureForward = transform.forward + rigidbody.velocity * 0.2f;

        float futureCarPosition = 0f;

        spline.FindNearestPointTo(posFuturo, out futureCarPosition, 1000f);

        Vector3 futureTarget = spline.GetPoint(targetPosition);

        float futurebrakeLocationT = GetOffsetTCustom(futureCarPosition, BrakingDistance(0f));

        float futureCurvaturaDot = Mathf.Min(Vector3.Dot(futureForward.normalized, spline.GetTangent(futurebrakeLocationT).normalized), Vector3.Dot(futureForward.normalized, (futureTarget - posFuturo).normalized));

        return futureCurvaturaDot;
    }

    private void FixedUpdate()
    {
        Profiler.BeginSample("AI Spline codigo");

        //world space do nosso para valor 0 a 1
        Vector3 currentPosSpline = Vector3.zero;

        currentPosSpline  = spline.FindNearestPointTo(transform.position, out carPosition, 1000f, lastT);

        //target a minimo 12m de distancia
        targetPosition = GetOffsetT(true,12f);
        //targetPosition = carPosition;

        //ponto 3d desse 0,1
        target = spline.GetPoint(targetPosition);

        if (debugObj!=null)
        {
            debugObj.transform.position = target;
            debugObj2.transform.position = currentPosSpline;
        }

        //aceleraçao base
        vehicle.input.Vertical = 1f;

        brakeLocationT = GetOffsetT(false, BrakingDistance(0f));

        brakeLocation = GetSplinePoint(brakeLocationT);

        //VIRAR
        Vector3 sv = transform.InverseTransformPoint(new Vector3(target.x, transform.position.y, target.z));
        vehicle.input.Horizontal = Mathf.Clamp((sv.x / sv.magnitude) * 3.5f, -1f, 1f);


        //angulo entre targetVirar e nos
        float lateralAngle = Vector3.Angle(target-transform.position, transform.forward);

        if (debug)
        {
            debugText.text = "SteerTarget angle: " + lateralAngle.ToString("F2");


            debugText.text += "\nWheel Angle: " + vehicle.steering.Angle;
        }

        //maximo que pode virar a velocidade atual
        maxSteerAngle = Mathf.Abs(Mathf.Lerp(vehicle.steering.lowSpeedAngle, vehicle.steering.highSpeedAngle, vehicle.Speed / vehicle.steering.crossoverSpeed));

        if (debug)
            debugText.text += "\nMax Wheel Angle: " + maxSteerAngle;



        float lateralAngleBrake = Vector3.Angle(brakeLocation - transform.position, transform.forward);
        if (debug)
            debugText.text += "\nBrake Angle: " + lateralAngleBrake;


        float curvaturaTeuSteer = Mathf.Cos(maxSteerAngle);

        //distanciaAB * cos(AB)
        float curvaturaDot = Mathf.Min(Vector3.Dot(transform.forward.normalized,spline.GetTangent(brakeLocationT).normalized), Vector3.Dot(transform.forward.normalized, (target-transform.position).normalized));
        
        if (debug)
            debugText.text += "\nCurvatura: " + curvaturaDot;

        float futureCurvaturaDot = PreverPosicaoFuturo();

        if (debug)
            debugText.text += "\nCurvatura Futuro: " + futureCurvaturaDot;

        float cosAngle = Mathf.Cos(maxSteerAngle*Mathf.Deg2Rad);


        float medianCurvatura = (curvaturaDot + futureCurvaturaDot) / 2f;

        if (debug)
            debugText.text += "\nCurvatura Real: " + medianCurvatura;

        //brake
        float B = (cosAngle - medianCurvatura) / (cosAngle - (brakeAgressive));
        
        if (useNewCalc)
        {
            B = (cosAngle - medianCurvatura) / (cosAngle - (brakeAgressive)) * (BrakingDistance(0f) / Vector3.Distance(transform.position, brakeLocation));
        }
        
        B = Mathf.Clamp01(B);
        B = BlookUp.Evaluate(B);

        //acelerar
        float A = (medianCurvatura - cosAngle) / (1f - cosAngle);
        
        if (useNewCalc)
        {
            A = (medianCurvatura - cosAngle) / (1f - cosAngle) * (Vector3.Distance(transform.position, brakeLocation) / BrakingDistance(0f)) + 1f - vehicle.input.Horizontal + 1f - ((vehicle.axles[0].leftWheel.SideSlipPercent+ vehicle.axles[0].rightWheel.SideSlipPercent)/2f);
        }       
        
        A = Mathf.Clamp01(A)*dificuldade;
        A = Mathf.Clamp01(A);
        A = AlookUp.Evaluate(A);
        if (debug)
        {
            debugText.text += "\nBRAKE: " + B;
            debugText.text += "\nACELERAR: " + A;
        }      
        

        if (vehicle.SpeedKPH > 20f)
        {
            //final input
            vehicle.input.Vertical = A - B;
        }
        if (debug)
        {
            debugText.text += "\nINPUT FINAL: " + vehicle.input.Vertical;
        }

        lastT = carPosition;

        Profiler.EndSample();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            //steer angle
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(maxSteerAngle, transform.up) * transform.forward * 15f);
            Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-maxSteerAngle, transform.up) * transform.forward * 15f);


            //linha amarela
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + new Vector3(0f, 2f, 0f), (brakeLocation - transform.position) + new Vector3(0f, 2f, 0f));

            //frente do carro
            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position + new Vector3(0f, 2f, 0f), transform.forward * 8f);

            //tangente
            Gizmos.color = Color.red;
            Gizmos.DrawRay(brakeLocation + new Vector3(0f, 2f, 0f), spline.GetTangent(brakeLocationT)+ new Vector3(0f,2f,0f));

            //vetor velocidade
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + new Vector3(0f, 2f, 0f), rigidbody.velocity + new Vector3(0f, 2f, 0f));
        }
    }
}

public class Kinematic
{
    Vector3 position;
    float orientation;

    //linear velocity
    Vector3 velocity;

    //angular velocity
    float rotation;
}

class SteeringOutput
{
    Vector3 linear;
    float angular;
}
