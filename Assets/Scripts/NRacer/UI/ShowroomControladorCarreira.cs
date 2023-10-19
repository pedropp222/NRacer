using Assets.Scripts.NRacer.Controllers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowroomControladorCarreira : MonoBehaviour, ICarreiraPainelUI
{
    public List<GameObject> carrosAtuais = new List<GameObject>();
    public List<CarroPlayerData> carrosData = new List<CarroPlayerData>();

    public Transform posicaoCarroSpawn;

    int carroAtual = 0;

    public GameObject carPanel;

    public GameObject obterCarroButton;

    public Color[] raridadeCores;

    public RendercamaraSprite rendercamara;

    public GameObject botaoPrefab;

    public PainelVerticalScrollUI painelVertical;

    TipoShowroom tipoAtual;

    public enum TipoShowroom
    {
        concessionario,
        escolher,
        garagem
    }

    public void ApresentarShowroom(CarroPlayerData[] listaCarros, TipoShowroom tipoShowroom, CarroFiltro filtro)
    {
        carrosAtuais.Clear();
        carrosData.Clear();
        int index = 0;
        carroAtual = 0;

        //Instanciar e registar os carros que estao visiveis;
        foreach (CarroPlayerData x in listaCarros)
        {
            GameObject go = Instantiate(Controlador.instancia.carros[x.id], posicaoCarroSpawn.position, posicaoCarroSpawn.rotation);

            go.GetComponent<CarroStats>().CarregarTrim(x.trimId);

            carrosAtuais.Add(go);
            carrosData.Add(x);

            if (index == 0)
            {
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }

            GameObject btn = painelVertical.InstanciarElemento(botaoPrefab, 10f);

            btn.transform.GetChild(0).GetComponent<Text>().text = go.GetComponent<CarroStats>().NomeResumido(false);

            index++;
        }

        tipoAtual = tipoShowroom;

        IniciarCarro();
    }

    public void LimparShowroom()
    {
        //destruir carros do showroom
        foreach (GameObject x in carrosAtuais)
        {
            Destroy(x);
        }
        //carPanel.SetActive(false);
        carrosAtuais.Clear();
        carrosData.Clear();
        //bolas.Clear();
        painelVertical.Reset();
    }

    /// <summary>
    /// Mostrar o showroom pela primeira vez
    /// </summary>
    public void IniciarCarro()
    {
        //carroAtual = 1;

        //carrosAtuais[carroAtual].SetActive(true);
        //bolas[carroAtual].color = bolaSelectedCores[1];

        obterCarroButton.GetComponent<Button>().Select();

        AtualizarUI();
        StartCoroutine(SelectCarro(false));
    }


    bool aCorrer = false;
    /// <summary>
    /// Selecionar o proximo carro ou anterior, se possivel
    /// </summary>
    /// <param name="proximo">selecionar o proximo ou o anterior</param>
    public IEnumerator SelectCarro(bool proximo)
    {
        if (!aCorrer)
        {
            aCorrer = true;
            if (proximo)
            {
                if (carroAtual + 1 != carrosAtuais.Count)
                {
                    yield return new WaitForSeconds(0.3f);
                    carrosAtuais[carroAtual].SetActive(false);
                    carroAtual++;
                    carrosAtuais[carroAtual].SetActive(true);
                    AtualizarUI();
                }
            }
            else
            {
                if (carroAtual - 1 >= 0)
                {
                    yield return new WaitForSeconds(0.3f);
                    carrosAtuais[carroAtual].SetActive(false);
                    carroAtual--;
                    carrosAtuais[carroAtual].SetActive(true);
                    AtualizarUI();
                }
            }
            aCorrer = false;
        }
    }

    public void AtualizarUI()
    {
        carPanel.SetActive(true);

        CarroStats stats = carrosAtuais[carroAtual].GetComponent<CarroStats>();

        //nome completo do carro
        carPanel.transform.GetChild(1).GetComponent<Text>().text = "";
        carPanel.transform.GetChild(1).GetComponent<Text>().DOText(stats.NomeCompleto(), 0.25f);

        //105 HP | 1280 KG | TRAÇAO

        carPanel.transform.GetChild(2).GetComponent<Text>().text = "";
        carPanel.transform.GetChild(2).GetComponent<Text>().DOText(stats.GetPotencia() + " HP | " + stats.GetPeso() + " KG | " + stats.tracao.ToString(), 0.25f);

        //0-100

        carPanel.transform.GetChild(3).GetComponent<Text>().DOText(stats.trimAtual.zeroAos100 > 0f ? "0-100: " + stats.trimAtual.zeroAos100 + "s" : "0-100: Nao consegue :)", 0.65f);


        //odometer, se aplicavel

        /*if (tipoAtual == TipoShowroom.garagem)
        {
            carPanel.transform.GetChild(4).GetComponent<Text>().DOText(Mathf.Round(stats.quilometros / 1000f) + "KM", 0.65f);
        }
        else
        {
            carPanel.transform.GetChild(4).GetComponent<Text>().DOText("", 0.65f);
        }*/

        //raridade
        carPanel.transform.GetChild(5).GetComponent<Text>().DOText(stats.trimAtual.trimRaridade.ToString(), 0.65f);
        carPanel.transform.GetChild(5).GetComponent<Text>().color = raridadeCores[(int)stats.trimAtual.trimRaridade];

        //obter carro button

        obterCarroButton.GetComponent<Button>().onClick.RemoveAllListeners();

        if (tipoAtual == TipoShowroom.garagem)
        {

            obterCarroButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                Controlador.instancia.SetCarroSelected(carrosData[carroAtual]);
                //LimparShowroom();
            });
        }
        else if (tipoAtual == TipoShowroom.escolher)
        {
            obterCarroButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                Controlador.instancia.SetCarroSelected(carrosData[carroAtual]);
                Controlador.instancia.GerarCorridaCarreira();
                OnDesativar();
            });
        }
    }

    public void OnAtivar()
    {
        gameObject.SetActive(true);
        rendercamara.Aparecer();
    }

    public void OnDesativar()
    {
        LimparShowroom();
        gameObject.SetActive(false);
        rendercamara.Desaparecer();
    }
}
