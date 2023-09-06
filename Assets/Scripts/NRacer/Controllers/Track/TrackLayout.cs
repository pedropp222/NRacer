using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NRacer.Controllers.Track
{
    /// <summary>
    /// Classe para organizar informacao sobre um layout de uma pista, em que contem os spawns e os pontos a seguir do ai
    /// para este layout
    /// </summary>
    [System.Serializable]
    public class TrackLayout
    {
        public string layoutName;

        public GameObject spawnpointsParent;
        public GameObject waypointsAi;
    }
}
