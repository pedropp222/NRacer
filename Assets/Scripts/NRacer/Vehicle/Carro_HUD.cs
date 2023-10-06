using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// O HUD de um veiculo qualquer, mas na maioria dos carros do player
/// </summary>
public class Carro_HUD : MonoBehaviour
{
    NWH.VehiclePhysics.VehicleController carro;

    public RectTransform agulhaRPM;
    public Text veloKMH;
    public Text posicao;
    public Text volta;
    public Text gear;
    public int numVoltas;

    float newR;

    bool setup = false;

    public int pos=-1;

    private CarroVolta carroVolta;

    private void Start()
    {
        carro = GetComponent<NWH.VehiclePhysics.VehicleController>();
        carroVolta = GetComponent<CarroVolta>();
    }

    private void FixedUpdate()
    {
        if (!setup)
        {
            return;
        }

        veloKMH.text = string.Format("{0:0}",carro.SpeedKPH)+" KM/H";
        gear.text = carro.transmission.Gear == 0 ? "N" : carro.transmission.Gear < 0 ? "R" : carro.transmission.Gear.ToString();

        Vector3 rotAgulha = agulhaRPM.rotation.eulerAngles;

        newR = Mathf.Lerp(newR, carro.engine.RPM, 0.3f);

        rotAgulha.z = ((newR * 180f) / carro.engine.maxRPM) * -1f;

        agulhaRPM.eulerAngles = rotAgulha;

        volta.text = "Volta: "+(carroVolta.voltas+1).ToString()+"/"+numVoltas;

    }

    /// <summary>
    /// Atualizar a posicao atual no UI
    /// </summary>
    public void ReceberPosicao(int pos)
    {
        this.pos = pos;
        if (posicao == null) return;
        posicao.text = pos + "º";      
    }

    /// <summary>
    /// Fazer o setup do HUD para este veiculo, fornecendo os objetos necessarios
    /// </summary>
    public void ReceberHUD(RectTransform rect, Text velo, Text pos, Text volta, Text gear, int numVoltas)
    {
        agulhaRPM = rect;
        veloKMH = velo;
        posicao = pos;
        this.volta = volta;
        this.numVoltas = numVoltas;
        this.gear = gear;
        setup = true;
    }

    public void StopHUD()
    {
        setup = false;
    }
}