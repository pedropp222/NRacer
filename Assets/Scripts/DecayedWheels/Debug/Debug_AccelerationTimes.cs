using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.VehiclePhysics;

/// <summary>
/// Logger da velocidade do veiculo em varios momentos
/// util para saber tempo de aceleraçao de x ate y ex: 0,100km/h
/// </summary>
public class Debug_AccelerationTimes : MonoBehaviour
{
    bool start = false;

    VehicleController vehicle;

    float initialTime = 0f;

    List<Logger> loggers;

    private void Start()
    {
        vehicle = GetComponent<VehicleController>();
        loggers = new List<Logger>();

        for(int i = 20; i < 250; i+=10)
        {
            loggers.Add(new Logger(i));
        }
    }

    private void Update()
    {
        if (start) return;

        if (vehicle.input.Vertical>0.8f)
        {
            if (!start)
            {
                start = true;
                initialTime = Time.timeSinceLevelLoad;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!start) return;


        TryLogging(vehicle.SpeedKPH);

    }

    void TryLogging(float speed)
    {
        for(int i = 0; i < loggers.Count; i++)
        {
            if (speed >= loggers[i].speed)
            {
                if (loggers[i].time == -1f)
                {
                    loggers[i].time = Time.timeSinceLevelLoad - initialTime;

                    Debug.Log("0-" + loggers[i].speed + ": " + loggers[i].time.ToString("F1"));
                }
            }
        }
    }
}


public class Logger
{
    public float speed;
    public float time;

    public Logger(float sp)
    {
        time = -1f;
        speed = sp;
    }
}