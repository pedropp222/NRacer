using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Classe que vai ser usada para controlar o showroom de carros. Controla a lista de carros
/// e o UI da informação e interatividade possivel (comprar, vender, escolher, etc.)
/// </summary>
public class ShowroomControlador : MonoBehaviour
{
    bool ativo = false;

    public List<GameObject> carrosAtuais = new List<GameObject>();

    public Transform posicaoCarroSpawn;

    int carroAtual = 0;

    public GameObject carPanel;

    public GameObject obterCarroButton;


    public GameObject bolaPrefab;
    public Transform gridBolas;


    public Color[] raridadeCores;

    public Color[] bolaSelectedCores;

    TipoShowroom tipoAtual;

    public List<Image> bolas = new List<Image>();

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
    public void ApresentarShowroom(GameObject[] listaCarros, TipoShowroom tipoShowroom)
    {
        //Instanciar e registar os carros que estao visiveis;
        foreach(GameObject x in listaCarros)
        {
            GameObject go = Instantiate(x, posicaoCarroSpawn.position, posicaoCarroSpawn.rotation);
            carrosAtuais.Add(go);
            go.SetActive(false);
            bolas.Add(Instantiate(bolaPrefab,gridBolas).GetComponent<Image>());
        }

        tipoAtual = tipoShowroom;

        IniciarCarro();

        ativo = true;
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
    }

    /// <summary>
    /// Mostrar o showroom pela primeira vez
    /// </summary>
    public void IniciarCarro()
    {
        carroAtual = 0;

        carrosAtuais[carroAtual].SetActive(true);
        bolas[carroAtual].color = bolaSelectedCores[1];

        carPanel.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => SelectCarro(false));
        carPanel.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => SelectCarro(true));

        AtualizarUI();
    }

    /// <summary>
    /// Selecionar o proximo carro ou anterior, se possivel
    /// </summary>
    /// <param name="proximo">selecionar o proximo ou o anterior</param>
    public void SelectCarro(bool proximo)
    {
        if (proximo)
        {
            if (carroAtual+1!=carrosAtuais.Count)
            {
                carrosAtuais[carroAtual].SetActive(false);
                bolas[carroAtual].color = bolaSelectedCores[0];
                carroAtual++;
                carrosAtuais[carroAtual].SetActive(true);
                bolas[carroAtual].color = bolaSelectedCores[1];
                AtualizarUI();
                return;
            }
        }
        else
        {
            if (carroAtual - 1 >= 0)
            {
                carrosAtuais[carroAtual].SetActive(false);
                bolas[carroAtual].color = bolaSelectedCores[0];
                carroAtual--;
                carrosAtuais[carroAtual].SetActive(true);
                bolas[carroAtual].color = bolaSelectedCores[1];
                AtualizarUI();
                return;
            }
        }
    }

    public void AtualizarUI()
    {
        ///TODO
        ///aparecer o texto da informaçao do carro
        ///e aparecer os botoes com base no "modo" do showroom
        ///portanto no concessionario apresentar o botao "comprar", se for garagem, apresentar o botao "escolher",
        ///"vender", etc.
        ///

        carPanel.SetActive(true);

        CarroStats stats = carrosAtuais[carroAtual].GetComponent<CarroStats>();

        //nome completo do carro

        carPanel.transform.GetChild(0).GetComponent<Text>().text = stats.NomeCompleto();

        //105 HP | 1280 KG | TRAÇAO

        carPanel.transform.GetChild(1).GetComponent<Text>().text = stats.potencia + " HP | " + stats.peso + " KG | " + stats.tracao.ToString();

        //0-100

        carPanel.transform.GetChild(2).GetComponent<Text>().text = "0-100: " + stats.zeroAos100 + "s";


        //odometer, se aplicavel

        if (tipoAtual== TipoShowroom.garagem)
        {
            carPanel.transform.GetChild(3).GetComponent<Text>().text = Mathf.Round(stats.quilometros/1000f)+"KM";
        }
        else
        {
            carPanel.transform.GetChild(3).GetComponent<Text>().text = "";
        }

        //raridade
        carPanel.transform.GetChild(4).GetComponent<Text>().text = stats.raridade.ToString();
        carPanel.transform.GetChild(4).GetComponent<Text>().color = raridadeCores[(int)stats.raridade];

        //obter carro button

        obterCarroButton.GetComponent<Button>().onClick.RemoveAllListeners();
        obterCarroButton.GetComponent<Button>().onClick.AddListener(()=>FindObjectOfType<Controlador>().DarCarro(carroAtual, CarroStats.MetodoAquisicao.Arcade));
    }
}
