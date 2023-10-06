using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NWH.VehiclePhysics;
using DG.Tweening.Plugins;
using System.Collections.Generic;
using Assets.Scripts.NRacer.Vehicle;
using NWH.WheelController3D;
using System.Linq;

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
    
    public MetodoAquisicao metodoAquisicao;

    [HideInInspector] public float quilometros;
    
    public Sprite carroSprite;

    //Usado para calculo teorico da velocidade maxima
    public int tireHeightRatio;

    private float rodaCircumferencia;

    public int trimDefault;

    public CarroTrim trimAtual;
    public int trimAtualId;

    //valores default nivel 1 do carro
    private int pesoDefault;
    private float potenciaDefault;
    private float gearMultipierDefault;


    //valores atuais do nivel do carro
    private int pesoAtual;
    private float potenciaAtual;

    private float veloMaxima;

    public int carroNivel = 1;

    private VehicleController veiculo;
    private Rigidbody rigid;
    private float desempenhoAtual;

    private void Awake()
    {
        //se for o menu, ou cenario de desbloquear carro, 
        //desativar tudo, para ainda assim instanciar o veiculo e ele nao começar a andar
        //nem ter fisica nem nada disso.

        trimAtualId = -1;

        veiculo = GetComponent<VehicleController>();
        rigid = GetComponent<Rigidbody>();


        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
        {
            veiculo.Active = false;
            veiculo.enabled = false;
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
           
        }



        //Pista de testes
        //Estes 2 valores sao provisoriamente carregados a partir destes componentes.
        //Caso o trim contenha valores entao estes serao atualizados conforme
        if (trimAtual == null)
        {
            pesoDefault = (int)rigid.mass;
            potenciaDefault = veiculo.engine.maxPower * 1.3f;

            CarregarTrim(trimDefault);
        }
    }

    private void CalcularVelocidadeMaxima()
    {
        WheelController roda = transform.GetComponentInChildren<WheelController>();

        float rodaInch = roda.tireRadius * 100f * 2f / 3f;
        rodaCircumferencia = Mathf.PI * (rodaInch * 25.4f + (((roda.tireWidth * 1000f) * tireHeightRatio) / 100f) * 2f);

        veloMaxima = (((veiculo.engine.maxRPM / veiculo.transmission.ForwardGears.Last() / veiculo.transmission.gearMultiplier) * rodaCircumferencia) / 1000000f) * 60f;

        Debug.Log("Velocidade maxima teorica para este carro: " + veloMaxima);
    }

    public float GetPontosDesempenho()
    {
        return (potenciaAtual * (7500f - pesoAtual) + (veloMaxima * 14f)) / (trimAtual.zeroAos100 / 5f) / 10000f;
    }

    //TODO: Os trims vao ter que ter toda a informacao necessaria para aquele trim
    //e vai ter que carregar sempre toda a informacao. Entao os trims vao ter sempre peso, potencia, gear ratios, power curve, entre outros
    //de forma a que facilmente possamos visualizar a informacao total do veiculo em cada scriptable object, senao fica confuso ter defaults
    public void CarregarTrim(int trimId)
    {
        //Debug.Log("Tentar carregar trimId: " + trimId);

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

        if (trimAtual.peso > 0)
        {
            rigid.mass = trimAtual.peso;
            pesoDefault = trimAtual.peso;
            pesoAtual = pesoDefault;
        }

        if (trimAtual.potenciaKW > 0)
        {
            AtualizarPotenciaHP();
            potenciaDefault = potenciaAtual;
            veiculo.engine.maxPower = trimAtual.potenciaKW;
        }

        trimAtualId = trimId;

        gearMultipierDefault = veiculo.transmission.gearMultiplier;

        Debug.Log("Carregou o carro: " + NomeCompleto());
        //Debug.Log("PESO ATUAL: " + GetComponent<Rigidbody>().mass);
        CalcularVelocidadeMaxima();
        desempenhoAtual = GetPontosDesempenho();
        Debug.Log("PONTUACAO: " + desempenhoAtual + " - " + trimAtual.GetDesempenhoTexto(desempenhoAtual));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SubirNivel();
        }
    }

    public void SubirNivel()
    {
        if (carroNivel == 10)
        {
            //Ja esta no nivel maximo
            return;
        }

        if (carroNivel == 1) 
        {
            Debug.Log("Stats original de nivel " + carroNivel + " - Potencia Atual: " + potenciaAtual + " - Peso Atual: " + pesoAtual);
            Debug.Log("PONTUACAO NIVEL 1: " + desempenhoAtual + " - " + trimAtual.GetDesempenhoTexto(desempenhoAtual));
        }

        carroNivel++;

        switch (carroNivel)
        {
            case 2:
                veiculo.engine.maxPower = trimAtual.potenciaKW * (1f + trimAtual.upgradeIncrementarPotencia * 1f);
                AtualizarPotenciaHP();
                break;
            case 3:
                rigid.mass = trimAtual.peso / (1f + trimAtual.upgradeDecrementarPeso * 1f);
                pesoAtual = (int)rigid.mass;
                break;
            case 4:
                veiculo.transmission.gearMultiplier = gearMultipierDefault / (1f + trimAtual.upgradeIncrementarVeloMaxima * 1f);
                CalcularVelocidadeMaxima();
                break;
            case 5:
                veiculo.engine.maxPower = trimAtual.potenciaKW * (1f + trimAtual.upgradeIncrementarPotencia * 2f);
                AtualizarPotenciaHP();
                break;
            case 6:
                rigid.mass = trimAtual.peso / (1f + trimAtual.upgradeDecrementarPeso * 2f);
                pesoAtual = (int)rigid.mass;
                break;
            case 7:
                veiculo.transmission.gearMultiplier = gearMultipierDefault / (1f + trimAtual.upgradeIncrementarVeloMaxima * 2f);
                CalcularVelocidadeMaxima();
                break;
            case 8:
                veiculo.engine.maxPower = trimAtual.potenciaKW * (1f + trimAtual.upgradeIncrementarPotencia * 3f);
                AtualizarPotenciaHP();
                break;
            case 9:
                rigid.mass = trimAtual.peso / (1f + trimAtual.upgradeDecrementarPeso * 3f);
                pesoAtual = (int)rigid.mass;
                break;
            case 10:
                veiculo.transmission.gearMultiplier = gearMultipierDefault / (1f + trimAtual.upgradeIncrementarVeloMaxima * 3f);
                CalcularVelocidadeMaxima();
                break;
        }

        Debug.Log("Veiculo upgraded para nivel " + carroNivel + " - Potencia Atual: " + veiculo.engine.maxPower * 1.3f + " - Peso Atual: " + rigid.mass + " - Velo Max: "+ veloMaxima);
        desempenhoAtual = GetPontosDesempenho();
        Debug.Log("PONTUACAO: " + desempenhoAtual + " - " + trimAtual.GetDesempenhoTexto(desempenhoAtual));
    }

    private void AtualizarPotenciaHP()
    {
        potenciaAtual = veiculo.engine.maxPower * (veiculo.engine.forcedInduction.useForcedInduction ? 1.0f+veiculo.engine.forcedInduction.maxPowerGainMultiplier : 1.0f) * 1.3f;
    }

    public int GetPeso()
    {
        return pesoAtual;
    }

    public int GetPesoTrim(int trimId = -1)
    {
        return modelo.trimsDisponiveis[trimId].peso;
    }

    public float GetPotencia()
    {
        return potenciaAtual;
    }

    public float GetPotenciaTrim(int trimId = -1)
    {
        return modelo.trimsDisponiveis[trimId].potenciaKW*1.3f;
    }

    public int GetAno()
    {
        return modelo.ano;
    }

    public string NomeCompleto()
    {
        return GetNomeTrim(trimAtualId);
    }

    public string NomeResumido(bool ai)
    {
        if (ai)
        {
            return marca.nome + " " + modelo.nome + " (" + dificuldadeAI + ")";
        }
        return marca.nome + " " + modelo.nome;
    }

    public int GetPreco()
    {
        //return Mathf.RoundToInt(((getPontos()*velocidadeMaxima)/zeroAos100+((3500f-peso)/1.2f))*2.15f);
        return trimAtual.preco;
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