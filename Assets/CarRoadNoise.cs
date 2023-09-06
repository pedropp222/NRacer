using NWH.VehiclePhysics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Security.Policy;
using UnityEngine;

public class CarRoadNoise : MonoBehaviour
{
    public AudioClip clip;

    public float maxVolume;

    public float maxVolumeSpeed;

    private AudioSource source;

    private VehicleController vehicle;

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0f;
        source.loop = true;
        source.minDistance = 6f;
        source.maxDistance = 35f;
        source.spatialBlend = 1.0f;
        source.Play();
        vehicle = GetComponent<VehicleController>();
    }

    private void FixedUpdate()
    {
        source.volume = Mathf.Lerp(0f, maxVolume, Mathf.Clamp01(Mathf.Pow(vehicle.SpeedKPH / maxVolumeSpeed,1.5f)));
    }
}
