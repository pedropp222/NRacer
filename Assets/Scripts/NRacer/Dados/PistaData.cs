using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NRacer.Dados
{
    [CreateAssetMenu(fileName = "PistaDados", menuName = "JogoDados/PistaDados", order = 11)]
    public class PistaData : ScriptableObject
    {
        public int nivelId;
        public string pistaNome;

        [SerializeField] public LayoutData[] layouts;
    }

    [System.Serializable]
    public struct PistaInfo
    {
        public int nivelId;
        public int layoutId;

        public PistaInfo(int nivelId, int layoutId)
        {
            this.nivelId = nivelId;
            this.layoutId = layoutId;
        }
    }


    [System.Serializable]
    public struct LayoutData
    {
        public Sprite pistaImagem;
        public string layoutNome;
        public PistaTipo tipoPista;
        public EstradaTipo tipoEstrada;
    }

    public enum PistaTipo
    {
        CIRCUITO,
        DRAG,
        SPRINT
    }

    public enum EstradaTipo
    {
        ASFALTO,
        TERRA,
        AMBOS
    }

}
