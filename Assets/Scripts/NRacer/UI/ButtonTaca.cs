using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// Colocar um icone de taçinha de acordo se o player ganhou essa corrida ou nao
/// </summary>
public class ButtonTaca : MonoBehaviour
{
    public int campeonatoID;
    public int corridaID;

    private void Start()
    {    
        /*if (Controlador.campeonatos.campeonatos[campeonatoID].ganhos[corridaID])
        {
            transform.GetChild(2).gameObject.SetActive(true);
        }*/
    }
}