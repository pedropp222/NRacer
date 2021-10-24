using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NWH.VehiclePhysics;

/// <summary>
/// Classe que contem as stats básicas dos carros, num local facil de acessar
/// Contem marca, modelo, peso, potencia, traçao e muitos outros aspetos deste veiculo num so local
/// </summary>
public class CarroStats : MonoBehaviour
{
    public enum Marca
    {
        Alfa_Romeo,
        Audi,
        Autobianchi,
        Chevrolet,
        Datsun,
        Dodge,
        Ford,
        Honda,
        Jaguar,
        Lancia,
        Mitsubishi,
        Piaggio,
        Panhard,
        Peugeot,
        Renault,
        Toyota,
        Volkswagen
    }

    public enum Tracao
    {
        frente,
        tras,
        x4
    }

    public enum Raridade
    {
        comum,
        incomum,
        raro,
        especial,
        lendario
    }

    public enum MetodoAquisicao
    {
        Concessionario,
        Usado,
        Leilao,
        Premio,
        Arcade
    }

    public enum Dificuldade
    {
        Muito_Facil,
        Facil,
        Medio,
        Dificil,
        Muito_Dificil
    }

    public Marca marca;
    public string modelo;
    public int ano;
    public int peso;
    public Dificuldade dificuldadeAI;
    public Tracao tracao;
    public Raridade raridade;
    public int preco;
    public MetodoAquisicao metodoAquisicao;
    public float quilometros;
    public float zeroAos100;
    public int velocidadeMaxima;
    public int potencia;
    public Sprite carroSprite;

    private void Awake()
    {
        //se for o menu, ou cenario de desbloquear carro, 
        //desativar tudo, para ainda assim instanciar o veiculo e ele nao começar a andar
        //nem ter fisica nem nada disso.

        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
        {
            NWH.VehiclePhysics.VehicleController car = GetComponent<NWH.VehiclePhysics.VehicleController>();
            car.Active = false;
            car.enabled = false;
            GetComponent<VehicleAI>().enabled = false;
            GetComponent<Carro_HUD>().enabled = false;
            GetComponent<CarroVolta>().enabled = false;
            if (GetComponent<CameraOffset>() != null)
            {
                GetComponent<CameraOffset>().enabled = false;
            }
            GetComponent<Rigidbody>().isKinematic = true;
            return;
        }
    }

    void Start() 
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            peso = (int)GetComponent<Rigidbody>().mass;
            potencia = Mathf.RoundToInt(GetComponent<VehicleController>().engine.Power);
        }
    }

    public int GetPeso()
    {
        if (peso == 0) 
        {
            peso = (int)GetComponent<Rigidbody>().mass;
        }
        return peso;
    }

    public int GetPotencia()
    {
        if(potencia == 0) 
        {
            potencia = Mathf.RoundToInt(GetComponent<VehicleController>().engine.maxPower);
        }
        return potencia;
    }

    public string NomeCompleto()
    {
        return marca + " " + modelo + " " + ano;
    }

    public string NomeResumido(bool ai)
    {
        if (ai)
        {
            return modelo + " (" + dificuldadeAI + ")";
        }
        return modelo;
    }

    public int GetPontos()
    {
        ///TODO - CRIAR UM ALGORITMO PARA DEFINIR PONTUAÇAO GERAL DA CAPACIDADE DOS CARROS
        return Mathf.RoundToInt(zeroAos100 + velocidadeMaxima - (peso / 70));
    }

    public int GetPreco()
    {
        //return Mathf.RoundToInt(((getPontos()*velocidadeMaxima)/zeroAos100+((3500f-peso)/1.2f))*2.15f);
        return preco;
    }
}