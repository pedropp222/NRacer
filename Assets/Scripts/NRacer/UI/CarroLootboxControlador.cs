using Assets.Scripts.NRacer.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.Scripts.NRacer.GameMode.Career;

public class CarroLootboxControlador : MonoBehaviour, ICarreiraPainelUI
{
    public Color[] raridadeCores;

    public RendercamaraSprite rendercamara;

    public Text raridadeText;
    public Text carroNomeText;
    public Text carroStatsText;
    public Text carroVeloText;
    public Text carroPontosDText;

    public Button botaoCarro;

    public Image fadeImg;

    public void IniciarLootbox(CarroFiltro filtro)
    {
        if (!gameObject.activeSelf)
        {
            OnAtivar();
        }

        CarroData cd = Controlador.instancia.ObterCarroFiltro(filtro);

        StartCoroutine(LootboxSequencia(cd));
    }

    public void IniciarLootbox(CarroData carro)
    {
        if (!gameObject.activeSelf)
        {
            OnAtivar();
        }

        StartCoroutine(LootboxSequencia(carro));
    }

    public void OnAtivar()
    {
        gameObject.SetActive(true);

        botaoCarro.gameObject.SetActive(false);

        raridadeText.text = "";
        carroNomeText.text = "";
        carroStatsText.text = "";
        carroVeloText.text = "";
        carroPontosDText.text = "";

        rendercamara.Desaparecer();
    }

    public void OnDesativar()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator LootboxSequencia(CarroData carro)
    {
        
        CarroStats stats = Controlador.instancia.InstanciarCarro(carro).GetComponent<CarroStats>();
       
        stats.CarregarTrim(carro.trimId);

        Debug.Log("Carregou carro: " + carro.id + "," + carro.trimId);

        //raridade
        raridadeText.text = stats.trimAtual.trimRaridade.ToString();
        raridadeText.color = raridadeCores[(int)stats.trimAtual.trimRaridade];

        yield return new WaitForSeconds(0.6f);        

        carroPontosDText.text = "Pontos de Desempenho: " + stats.GetPontosDesempenho();

        yield return new WaitForSeconds(0.6f);

        //ano do carro
        carroNomeText.text = stats.GetAno().ToString();

        yield return new WaitForSeconds(0.6f);

        //105 HP | 1280 KG | TRAÇAO

        carroStatsText.text = stats.GetPotencia() + " HP ";

        yield return new WaitForSeconds(0.6f);

        carroStatsText.text = stats.GetPotencia() + " HP | " + stats.GetPeso() + " KG ";

        yield return new WaitForSeconds(0.6f);

        carroStatsText.text = stats.GetPotencia() + " HP | " + stats.GetPeso() + " KG | Tração:" + stats.tracao.ToString();

        yield return new WaitForSeconds(0.6f);
        
        carroVeloText.text=stats.trimAtual.zeroAos100 > 0f ? "0-100: " + stats.trimAtual.zeroAos100 + "s" : "0-100: Nao consegue :)";       

        yield return new WaitForSeconds(0.8f);

        rendercamara.Aparecer();

        //nome completo do carro
        carroNomeText.text = "";
        yield return carroNomeText.DOText(stats.NomeCompleto(), 0.65f).WaitForKill();      

        yield return new WaitForSeconds(0.3f);

        botaoCarro.gameObject.SetActive(true);

        CarroPlayerData playerCarro = new CarroPlayerData(carro.id,carro.trimId,stats.GetComponent<VehicleRandomSkin>().corEscolhida);

        botaoCarro.GetComponent<Button>().onClick.RemoveAllListeners();
        botaoCarro.GetComponent<Button>().onClick.AddListener(() => { 
            Controlador.instancia.ObterCarro(playerCarro); 
            ModoCarreiraUI.instancia.SetBotoesTop(true); 
            ModoCarreiraUI.instancia.DesativarTodosPaineis(); 
            rendercamara.Desaparecer(); 
        });

    }
}
