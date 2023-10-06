using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.NRacer.Controllers;
using DG.Tweening;

/// <summary>
/// Classe que vai ser usada para controlar o showroom de carros. Controla a lista de carros
/// e o UI da informação e interatividade possivel (comprar, vender, escolher, etc.)
/// </summary>
public class ShowroomControlador : MonoBehaviour
{
    bool ativo = false;

    public List<GameObject> carrosAtuais = new List<GameObject>();
    public List<CarroData> carrosData = new List<CarroData>();

    public Transform posicaoCarroSpawn;

    int carroAtual = 0;

    public GameObject carPanel;

    public GameObject obterCarroButton;
    public Image fadeImg;


    public GameObject bolaPrefab;
    public Transform gridBolas;

    public Color[] raridadeCores;

    public Color[] bolaSelectedCores;

    TipoShowroom tipoAtual;

    public List<Image> bolas = new List<Image>();

    public GameObject objetoUiVinculo;

    public enum TipoShowroom
    {
        concessionario,
        escolher,
        garagem
    }

    /// <summary>
    /// Criar um showroom com uma lista pre determinada de carros
    /// </summary>
    /// <param name="listaCarros">A lista dos carros para apresentar</param>
    /// 

    //TODO: Aqui no showroom temos que melhorar a usabilidade, as transicoes on, off e de selecionar carros
    //Alem disso tem que ser fazer um sistema de paginas para nao ter que instanciar todos os carros de uma so vez
    public void ApresentarShowroom(GameObject[] listaCarros, TipoShowroom tipoShowroom)
    {
        carrosAtuais.Clear();
        carrosData.Clear();
        int index = 0;
        carroAtual = 0;

        FindAnyObjectByType<MenuUI>().SetMenuBackground(false);
        objetoUiVinculo.GetComponent<UIPainelEventos>().DesativarPainel();

        //Instanciar e registar os carros que estao visiveis;
        foreach (GameObject x in listaCarros)
        {
            GameObject go = Instantiate(x, posicaoCarroSpawn.position, posicaoCarroSpawn.rotation);
            CarroStats carro = go.GetComponent<CarroStats>();        

            carrosAtuais.Add(go);
            carrosData.Add(new CarroData(index, 0));
            go.SetActive(false);
            bolas.Add(Instantiate(bolaPrefab,gridBolas).GetComponent<Image>());

            if (carro.modelo.trimsDisponiveis.Length > 1)
            {
                //o trim default 0 e sempre instanciado, temos que comecar no ID 1 para carregar os restantes
                for (int i = 1; i < carro.modelo.trimsDisponiveis.Length; i++)
                {
                    GameObject go2 = Instantiate(x, posicaoCarroSpawn.position, posicaoCarroSpawn.rotation);
                    CarroStats carro2 = go2.GetComponent<CarroStats>();
                    carro2.CarregarTrim(i);
                    carrosAtuais.Add(go2);
                    carrosData.Add(new CarroData(index, i));
                    go2.SetActive(false);
                    bolas.Add(Instantiate(bolaPrefab, gridBolas).GetComponent<Image>());
                }
            }
            index++;
        }

        tipoAtual = tipoShowroom;

        IniciarCarro();

        ativo = true;
    }

    public void IniciarShowroomLivre()
    {
        ApresentarShowroom(Controlador.instancia.carros, TipoShowroom.escolher);
    }

    public void LimparShowroom()
    {
        //destruir carros do showroom
        foreach (GameObject x in carrosAtuais)
        {
            Destroy(x);
        }

        //destruir bolas do ui
        for (int i = 0; i < gridBolas.childCount; i++)
        {
            Destroy(gridBolas.GetChild(i).gameObject);
        }

        ativo = false;
        carPanel.SetActive(false);
        carrosAtuais.Clear();
        carrosData.Clear();
        bolas.Clear();
    }

    /// <summary>
    /// Mostrar o showroom pela primeira vez
    /// </summary>
    public void IniciarCarro()
    {
        carroAtual = 1;

        //carrosAtuais[carroAtual].SetActive(true);
        bolas[carroAtual].color = bolaSelectedCores[1];

        carPanel.transform.GetChild(6).GetComponent<Button>().onClick.RemoveAllListeners();
        carPanel.transform.GetChild(7).GetComponent<Button>().onClick.RemoveAllListeners();

        carPanel.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SelectCarro(false)));
        carPanel.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SelectCarro(true)));

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
                    fadeImg.DOColor(Color.black, 0.25f);
                    yield return new WaitForSeconds(0.3f);
                    carrosAtuais[carroAtual].SetActive(false);
                    bolas[carroAtual].color = bolaSelectedCores[0];
                    carroAtual++;
                    carrosAtuais[carroAtual].SetActive(true);
                    bolas[carroAtual].color = bolaSelectedCores[1];
                    fadeImg.DOColor(new Color(0f, 0f, 0f, 0f), 0.25f);
                    AtualizarUI();
                }
            }
            else
            {
                if (carroAtual - 1 >= 0)
                {
                    fadeImg.DOColor(Color.black, 0.25f);
                    yield return new WaitForSeconds(0.3f);
                    carrosAtuais[carroAtual].SetActive(false);
                    bolas[carroAtual].color = bolaSelectedCores[0];
                    carroAtual--;
                    carrosAtuais[carroAtual].SetActive(true);
                    bolas[carroAtual].color = bolaSelectedCores[1];
                    fadeImg.DOColor(new Color(0f, 0f, 0f, 0f), 0.25f);
                    AtualizarUI();
                }
            }
            aCorrer = false;
        }
    }

    public void AtualizarUI()
    {
        ///TODO: aparecer os botoes com base no "modo" do showroom
        ///portanto no concessionario apresentar o botao "comprar", se for garagem, apresentar o botao "escolher",
        ///"vender", etc.
        ///

        carPanel.SetActive(true);

        CarroStats stats = carrosAtuais[carroAtual].GetComponent<CarroStats>();

        //nome completo do carro

        //carPanel.transform.GetChild(0).GetComponent<Text>().text = stats.NomeCompleto();
        //carPanel.transform.GetChild(0).GetComponent<Text>().DOText(stats.NomeCompleto(),0.65f);
        carPanel.transform.GetChild(1).GetComponent<Text>().text = "";
        carPanel.transform.GetChild(1).GetComponent<Text>().DOText(stats.NomeCompleto(),0.25f);

        //105 HP | 1280 KG | TRAÇAO

        carPanel.transform.GetChild(2).GetComponent<Text>().text = "";
        carPanel.transform.GetChild(2).GetComponent<Text>().DOText(stats.GetPotencia() + " HP | " + stats.GetPeso() + " KG | " + stats.tracao.ToString(),0.25f);

        //0-100

        carPanel.transform.GetChild(3).GetComponent<Text>().DOText(stats.trimAtual.zeroAos100 > 0f ? "0-100: " + stats.trimAtual.zeroAos100 + "s" : "0-100: Nao consegue :)",0.65f);


        //odometer, se aplicavel

        if (tipoAtual== TipoShowroom.garagem)
        {
            carPanel.transform.GetChild(4).GetComponent<Text>().DOText(Mathf.Round(stats.quilometros / 1000f) + "KM", 0.65f);
        }
        else
        {
            carPanel.transform.GetChild(4).GetComponent<Text>().DOText("", 0.65f);
        }

        //raridade
        carPanel.transform.GetChild(5).GetComponent<Text>().DOText(stats.trimAtual.trimRaridade.ToString(), 0.65f);
        carPanel.transform.GetChild(5).GetComponent<Text>().color = raridadeCores[(int)stats.trimAtual.trimRaridade];

        //obter carro button

        //TODO: o botao de 'obter carro' tem que ser generificado se quisermos que isto seja o visualizador default dos veiculos em qualquer circunstancia

        obterCarroButton.GetComponent<Button>().onClick.RemoveAllListeners();
        obterCarroButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Tentar dar o carro selecionado, id: " + carrosData[carroAtual].id + "trim: " + stats.trimAtualId);
            //Controlador.instancia.DarCarro(carrosData[carroAtual].id, stats.trimAtualId, CarroStats.MetodoAquisicao.ARCADE);
            Controlador.instancia.SetCarroSelected(carrosData[carroAtual].id, stats.trimAtualId);
            FindAnyObjectByType<MenuUI>().SetMenuBackground(true);
            //objetoUiVinculo.SetActive(true);
            objetoUiVinculo.GetComponent<UIPainelEventos>().AtivarPainel();
            LimparShowroom();
        });
    }
}
