using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public Transform aroundObject;

    private void FixedUpdate() 
    {
        transform.RotateAround(aroundObject.position,Vector3.up,25f*Time.fixedDeltaTime);    
    }
}
