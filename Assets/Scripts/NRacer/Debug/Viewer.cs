using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace View
{

    public class Viewer : MonoBehaviour
    {
        public NWH.VehiclePhysics.VehicleController controller;

        private void FixedUpdate()
        {
            Debug.Log(controller.engine.Torque + "-" + controller.engine.RPM);
        }
    }
}
