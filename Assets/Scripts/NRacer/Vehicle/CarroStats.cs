using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NWH.VehiclePhysics;
using DG.Tweening.Plugins;
using System.Collections.Generic;
using Assets.Scripts.NRacer.Vehicle;

/// <summary>
/// Classe que contem as stats básicas dos carros, num local facil de acessar
/// Contem marca, modelo, peso, potencia, traçao e muitos outros aspetos deste veiculo num so local
/// </summary>
public class CarroStats : MonoBehaviour
{
    public enum Tracao
    {
        FRENTE,
        TRAS,
        X4
    }

    public enum MetodoAquisicao
    {
        CONCESSIONARIO,
        USADO,
        LEILAO,
        PREMIO,
        ARCADE
    }

    public enum Dificuldade
    {
        Muito_Facil,
        Facil,
        Medio,
        Dificil,
        Muito_Dificil
    }

    public CarroMarca marca;
    public CarroModelo modelo;
    public Dificuldade dificuldadeAI;
    public Tracao tracao;
    //TODO: O preco do carro vai depender do trim, nao pode ser global
    public int preco;
    public MetodoAquisicao metodoAquisicao;

    //TODO: Ver o melhor local para isto, porque isto depende do trim, e tambem no futuro irao existir upgrades
    public float quilometros;
    public float zeroAos100;
    public int velocidadeMaxima;

    public int potencia;
    public Sprite carroSprite;

    public int trimDefault;

    public CarroTrim trimAtual;
    public int trimAtualId;

    private int pesoDefault;

    private void Awake()
    {
        //se for o menu, ou cenario de desbloquear carro, 
        //desativar tudo, para ainda assim instanciar o veiculo e ele nao começar a andar
        //nem ter fisica nem nada disso.

        trimAtualId = -1;

        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
        {
            NWH.VehiclePhysics.VehicleController car = GetComponent<NWH.VehiclePhysics.VehicleController>();
            car.Active = false;
            car.enabled = false;
            if (GetComponent<VehicleAI>() != null)
            {
                GetComponent<VehicleAI>().enabled = false;
            }
            GetComponent<Carro_HUD>().enabled = false;
            GetComponent<CarroVolta>().enabled = false;
            if (GetComponent<CameraOffset>() != null)
            {
                GetComponent<CameraOffset>().enabled = false;
            }
            if (GetComponent<CarroCronometro>()!=null)
            {
                GetComponent<CarroCronometro>().enabled = false;
            }

            //Estes 2 valores sao provisoriamente carregados a partir destes componentes.
            //Caso o trim contenha valores entao estes serao atualizados conforme
            if (trimAtual == null)
            {
                pesoDefault = (int)GetComponent<Rigidbody>().mass;
                potencia = Mathf.RoundToInt(GetComponent<VehicleController>().engine.maxPower * 1.3f);

                CarregarTrim(trimDefault);
            }
            return;
        }
    }

    //TODO: Os trims vao ter que ter toda a informacao necessaria para aquele trim
    //e vai ter que carregar sempre toda a informacao. Entao os trims vao ter sempre peso, potencia, gear ratios, power curve, entre outros
    //de forma a que facilmente possamos visualizar a informacao total do veiculo em cada scriptable object, senao fica confuso ter defaults
    public void CarregarTrim(int trimId)
    {
        Debug.Log("Tentar carregar trimId: " + trimId);

        if (trimId == trimAtualId || trimId == -1)
        {
            return;
        }

        if (marca == null)
        {
            Debug.LogError("ERRO NO VEICULO: Nao tem nenhuma config de marca: " + gameObject.name);
            return;
        }

        if (modelo == null)
        {
            Debug.LogError("ERRO NO MODELO: Este veiculo nao tem configuracao do modelo: " + marca.nome);
            return;
        }

        if (modelo.trimsDisponiveis == null || modelo.trimsDisponiveis.Length == 0)
        {
            Debug.LogError("ERRO NOS TRIMS: Este veiculo nao tem nenhuma trim configurada: "+NomeResumido(false));
            return;
        }

        if (trimId >= modelo.trimsDisponiveis.Length)
        {
            Debug.Log("Nao conseguiu carregar o trimId " + trimId + ", pois so existem " + modelo.trimsDisponiveis.Length + " trims para este carro");
            return;
        }

        trimAtual = modelo.trimsDisponiveis[trimId];

        if (trimAtual.GetNovoPeso() != -1)
        {
            pesoDefault = trimAtual.GetNovoPeso();
            GetComponent<Rigidbody>().mass = pesoDefault;
        }

        if (trimAtual.GetNovaPotencia() != -1f) 
        {
            potencia = Mathf.RoundToInt(trimAtual.GetNovaPotencia() * 1.3f);
            GetComponent<VehicleController>().engine.maxPower = trimAtual.GetNovaPotencia();
        }

        trimAtualId = trimId;

        Debug.Log("Carregou o carro: " + NomeCompleto());
    }

    public int GetPeso()
    {
        if (pesoDefault == 0) 
        {
            pesoDefault = (int)GetComponent<Rigidbody>().mass;
        }
        return pesoDefault;
    }

    public int GetPesoTrim(int trimId = -1)
    {
        if (trimId == -1 || modelo.trimsDisponiveis[trimId].GetNovoPeso() < 0)
        {
            return GetPeso();
        }

        return modelo.trimsDisponiveis[trimId].GetNovoPeso();
    }

    public int GetPotencia()
    {
        if(potencia == 0) 
        {
            potencia = Mathf.RoundToInt(GetComponent<VehicleController>().engine.maxPower);
        }
        return potencia;
    }

    public float GetPotenciaTrim(int trimId = -1)
    {
        if (trimId == -1 || modelo.trimsDisponiveis[trimId].GetNovaPotencia() < 0f)
        {
            return GetPotencia();
        }

        return modelo.trimsDisponiveis[trimId].GetNovaPotencia();
    }

    public int GetAno()
    {
        return modelo.ano;
    }

    public string NomeCompleto()
    {
        return marca.nome + " " + modelo.nome + " " + trimAtual.nomeEspecial + " " + modelo.ano;
    }

    public string NomeResumido(bool ai)
    {
        if (ai)
        {
            return marca.nome + " " + modelo.nome + " (" + dificuldadeAI + ")";
        }
        return marca.nome + " " + modelo.nome;
    }

    public int GetPontos()
    {
        ///TODO - CRIAR UM ALGORITMO PARA DEFINIR PONTUAÇAO GERAL DA CAPACIDADE DOS CARROS
        return Mathf.RoundToInt(zeroAos100 + velocidadeMaxima - (pesoDefault / 70));
    }

    public int GetPreco()
    {
        //return Mathf.RoundToInt(((getPontos()*velocidadeMaxima)/zeroAos100+((3500f-peso)/1.2f))*2.15f);
        return preco;
    }

    /// <summary>
    /// Obter os nomes todos deste modelo de carro tendo em conta a quantidade de trims que aqui existem
    /// </summary>
    public string[] GetNomesPossiveisTrims()
    {
        List<string> nomes = new List<string>();

        for(int i = 0; i < modelo.trimsDisponiveis.Length; i++)
        {
            nomes.Add(GetNomeTrim(i));
        }

        return nomes.ToArray();
    }

    public string GetNomeTrim(int id)
    {
        if (id >= modelo.trimsDisponiveis.Length)
        {
            return "ND";
        }

        return marca.nome + " " + modelo.nome + (modelo.trimsDisponiveis[id].nomeEspecial.Length > 0 ? " " + modelo.trimsDisponiveis[id].nomeEspecial + " " : " ") + modelo.ano;
    }
}