using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Assets.Scripts.NRacer.Controllers;
using Assets.Scripts.NRacer.GameMode.Career;
using System.Data;
using Assets.Scripts.NRacer.Dados;

/// <summary>
/// Controlador massivo de tudo o que se passa no projeto, objeto que nunca é apagado e
/// que guarda tudo o que interessa no decorrer do jogo
/// </summary>
public class Controlador : MonoBehaviour
{
    static bool iniciar = true;

    [SerializeField]
    public CorridaInfo corridaAtual;
    public CorridaRules filtroAtual;

    public CorridaData corridaData = CorridaData.VAZIO;

    public GameObject[] carros;
    public GameObject[] aiCarros;

    public PistaData[] pistas;

    public GameObject carNameBox;

    public CarroPlayerData carroSelecionado = CarroPlayerData.Vazio;

    public GameMode modoAtual;

    //public static CampeonatosController campeonatos;

    public string nomePlayerAtual="";
    public SaveData saveDataPlayerAtual = null;

    public static Controlador instancia;

    public bool mudancasManuais;
    
    void Awake()
    {
        //singleton e tudo mais
        if (iniciar)
        {          
            iniciar = false;
            instancia = this;

            corridaAtual = new CorridaInfo(CorridaPremio.PremioDefault,-1,-1,-1);

            //abriu o jogo pela primeira vez!
            if (!Directory.Exists("savedata")|| NumeroJogadores() == 0)
            {
                Directory.CreateDirectory("savedata");
            }

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        Debug.Log("Encontrar e carregar um jogador qualquer");
        CarregarJogo();
    }

    public void CarregarJogo()
    {
        if (saveDataPlayerAtual==null||nomePlayerAtual.Length==0)
        {           
            saveDataPlayerAtual = GetPlayer(0);
            if(saveDataPlayerAtual==null)
            {
                Debug.LogWarning("Nao ha players ainda");
                MenuUI();
            }
            else
            {
                //campeonatos = saveDataPlayerAtual.corridas;
                nomePlayerAtual = saveDataPlayerAtual.PlayerName;
                Debug.Log("Carregou o player " + saveDataPlayerAtual.PlayerName);
                MenuUI();
            }
        }
        else
        {
            Debug.Log("Ja existe um jogador aberto com nome " + saveDataPlayerAtual.PlayerName);
        }
    }

    public void CarregarJogoGameMode(GameMode modoJogo)
    {
        //Debug.Log("Modo atual: " + modoAtual.GetType()+", mudar para "+modoJogo.GetType());

        /*if (modoAtual.GetType() == modoJogo.GetType())
        {
            Debug.Log("Ja estamos neste modo de jogo");
            return;
        }*/

        saveDataPlayerAtual = GetPlayer(nomePlayerAtual, modoJogo);
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 0 || arg0.buildIndex == 1)
        {
            //ao carregar o menu

            //se acabamos de participar numa corrida...
            //TODO: lol, tem que se arranjar uma forma bem melhor de saber que viemos de um evento que pode nem ser uma corrida
            if (corridaAtual.campeonatoID != -1)
            {
                //posicaofinal deve ser 0 no build
                if (corridaAtual.resultado.posicaoFinal == 0)
                {
                    Debug.Log("Corrida foi cancelada...");
                }
                else
                {
                    if (modoAtual != null)
                    {
                        modoAtual.ReceberResultadoCorrida(corridaAtual);
                        saveDataPlayerAtual.SaveGame();
                    }
                }

            }

            MenuUI();
        }
    }

    /// <summary>
    /// Refresh dos elementos atuais do ui
    /// </summary>
    //TODO: Isto secalhar nao vai ser preciso existir aqui!
    public void MenuUI()
    {
        MenuUI x = FindObjectOfType<MenuUI>();
        if (x!=null)
        {
            x.RefreshUI(this);
        }
    }

    /// <summary>
    /// Gera a corrida final, aplicando os filtros e definindo a informacao da mesma corrida, e inicia a scene
    /// </summary>
    /// <param name="filtros">Os filtros que controlam os carros que podem participar</param>
    /// <param name="information">Informaçao da corrida</param>
    public void GerarCorrida(CorridaRules filtros, CorridaInfo information)
    {
        corridaAtual = information;
        filtroAtual = filtros;

        CarroData[] gridPossivel = GetCarrosPossiveis(filtros);

        int numCarros = filtros.maxOponentes-1;

        if (numCarros > 0)
        {
            corridaAtual.startingGrid.AddRange(FiltrarPorRaridade(gridPossivel, numCarros));

            if (corridaAtual.startingGrid.Count == 0)
            {
                Debug.LogError("Nao consegue iniciar a corrida. Nao existem carros nenhuns depois de tentar filtrar os carros possiveis");
                return;
            }

            //Jogador começa em ultimo e tem o ID -1
            corridaAtual.startingGrid.Add(CarroData.Vazio);
        }

        Debug.Log("Corrida gerada, pronto a carregar");
        //SceneManager.LoadScene(filtros.nivel);
    }

    public void GerarCorridaCarreira()
    {
        if (corridaData == CorridaData.VAZIO)
        {
            Debug.LogError("Nao pode entrar numa corrida, porque nao existe contexto de corrida definida");
            return;
        }

        corridaAtual = new CorridaInfo(CorridaPremio.PremioDefault, corridaData.corridaRef.voltas, corridaData.campeonatoRef.id, corridaData.eventoIndex);
        filtroAtual = corridaData.corridaRef;

        GerarCorrida(filtroAtual, corridaAtual);

        SceneManager.LoadScene(filtroAtual.nivel.nivelId);
    }

    /// <summary>
    /// Gerar uma simples corrida numa pista aleatoria, com oponentes aleatorios
    /// Usa o carro que tens selecionado
    /// </summary>
    public void GerarCorridaLivre(int pistaId)
    {
        if (carroSelecionado == CarroPlayerData.Vazio)
        {
            Debug.LogError("Erro a gerar corrida livre. Nao tens carro selecionado");
            return;
        }

        GerarCorrida(CorridaRules.CorridaLivre(PistaAleatoria()), new CorridaInfo(CorridaPremio.PremioDefault, 2, 0, 0));
    }

    public PistaInfo PistaAleatoria()
    {
        PistaData pd = pistas[Random.Range(0, pistas.Length)];

        return new PistaInfo(pd.nivelId, Random.Range(0, pd.layouts.Length));
    }

    /// <summary>
    /// Obter a lista de carros possiveis atraves de regras de filtragem
    /// </summary>
    /// <param name="rules">Classe que contem as regras</param>
    /// <returns></returns>
    public CarroData[] GetCarrosPossiveis(CorridaRules rules)
    {
        List<CarroData> carrosLista = new List<CarroData>();

        for (int i = 0; i < carros.Length; i++)
        {
            CarroStats stats = carros[i].GetComponent<CarroStats>();

            if (rules.forceAICar != -1)
            {
                if (i==rules.forceAICar)
                {
                    carrosLista.Add(new CarroData(rules.forceAICar,0));
                }
            }
            else
            {
                if (stats.modelo == null || stats.modelo.trimsDisponiveis.Length == 0)
                {
                    Debug.LogError("ERRO A FILTRAR VEICULO: O veiculo com id " + i + " nao esta setup corretamente no CarroStats");
                    continue;
                }

                for (int k = 0; k < stats.modelo.trimsDisponiveis.Length; k++)
                {
                    if (!rules.filtroVeiculos.AvaliarPeso(stats.GetPesoTrim(k)))
                    {
                        continue;
                    }
                    if (!rules.filtroVeiculos.AvaliarPotencia((int)stats.GetPotenciaTrim(k)))
                    {
                        continue;
                    }
                    if (!rules.filtroVeiculos.AvaliarDesempenho((int)stats.GetPontosDesempenhoTrim(k)))
                    {
                        continue;
                    }
                    carrosLista.Add(new CarroData(i,k));
                }            
            }
        }

        Debug.Log("Existem " + carrosLista.Count + " carros possiveis para este evento");
        return carrosLista.ToArray();
    }

    /// <summary>
    /// Filtra uma lista de carros por raridade
    /// </summary>
    /// <param name="possiveis">Lista pre gerada dos ids dos carros possiveis</param>
    /// <param name="numCarros">Numero de carros no grid</param>
    /// <returns></returns>
    public CarroData[] FiltrarPorRaridade(CarroData[] possiveis, int numCarros)
    {
        List<CarroData> final = new List<CarroData>();

        if (possiveis.Length == 0)
        {
            Debug.LogError("Não existem carros possiveis para esta corrida!");
            return final.ToArray();
        }

        while(final.Count < numCarros)
        {
            CarroData carroEscolhido = CarroData.Vazio;
            while (carroEscolhido == CarroData.Vazio)
            {
                CarroData rc = possiveis[Random.Range(0, possiveis.Length)];             

                CarroTrim.Raridade raridade = carros[rc.id].GetComponent<CarroStats>().modelo.trimsDisponiveis[rc.trimId].trimRaridade;

                /*MUITO_COMUM,//F
                COMUM,//E
                MEDIANO,//D
                RARO,//C
                MUITO_RARO,//B
                ESPECIAL,//A
                UNICO//S*/

                if (raridade == CarroTrim.Raridade.UNICO)
                {
                    if (Random.Range(0, 55) == 32)
                    {
                        carroEscolhido = rc;
                    }
                }
                else if (raridade == CarroTrim.Raridade.ESPECIAL)
                {
                    if (Random.Range(0, 30) == 8)
                    {
                        carroEscolhido = rc;
                    }
                }
                else if (raridade == CarroTrim.Raridade.MUITO_RARO)
                {
                    if (Random.Range(0, 20) == 11)
                    {
                        carroEscolhido = rc;
                    }
                }
                else if (raridade == CarroTrim.Raridade.RARO)
                {
                    if (Random.Range(0,14) == 6)
                    {
                        carroEscolhido = rc;
                    }
                }
                else if (raridade == CarroTrim.Raridade.MEDIANO)
                {
                    if (Random.Range(0, 8) == 4)
                    {
                        carroEscolhido = rc;
                    }
                }
                else if (raridade == CarroTrim.Raridade.COMUM)
                {
                    if (Random.Range(0, 3) == 2)
                    {
                        carroEscolhido = rc;
                    }
                }
                else
                {
                    carroEscolhido = rc;
                }
            }

            final.Add(carroEscolhido);
        }

        return final.ToArray();
    }

    public CarroData ObterCarroFiltro(CarroFiltro filtro)
    {
        List<CarroData> carrosLista = new List<CarroData>();

        for(int i = 0; i < carros.Length; i++)
        {
            CarroStats stats = carros[i].GetComponent<CarroStats>();

            for (int k = 0; k < stats.modelo.trimsDisponiveis.Length; k++)
            {
                int peso = stats.GetPesoTrim(k);
                float potencia = stats.GetPotenciaTrim(k);
                float desempenho = stats.GetPontosDesempenhoTrim(k);

                if (filtro.usarPeso && (peso > filtro.maxPeso || peso < filtro.minPeso))
                {
                    continue;
                }
                if (filtro.usarPotencia && (potencia > filtro.maxPotencia || potencia < filtro.minPotencia))
                {
                    continue;
                }
                if (filtro.usarDesempenho && (desempenho > filtro.maxDesempenho || desempenho < filtro.minDesempenho))
                {
                    continue;
                }

                carrosLista.Add(new CarroData(i, k));
            }
        }

        return FiltrarPorRaridade(carrosLista.ToArray(), 1)[0];
    }

    public GameObject InstanciarCarro(CarroData carro)
    {
        GameObject go = Instantiate(carros[carro.id]);
        go.transform.position = Vector3.zero - new Vector3(0f,go.GetComponent<Rigidbody>().centerOfMass.y > 0f ? go.GetComponent<Rigidbody>().centerOfMass.y : 0f,0f);

        return go;
    }

    public void SetCarroSelected(CarroPlayerData carro)
    {
        carroSelecionado = carro;
    }

    public void SetCarroSelected(CarroData carro)
    {
        carroSelecionado = new CarroPlayerData(carro.id,carro.trimId);
    }

    public void ObterCarro(CarroPlayerData carro)
    {
        modoAtual.ObterCarro(carro);
        SetCarroSelected(carro);
    }

    public PistaData GetPistaByNivelId(int nivelId)
    {
        foreach(PistaData p in pistas)
        {
            if (p.nivelId == nivelId)
            {
                return p;
            }
        }

        return null;
    }

    /// <summary>
    /// Obter o numero de savegames
    /// </summary>
    /// <returns></returns>
    public int NumeroJogadores()
    {
        return Directory.GetDirectories("savedata").Length;
    }

    public void GravarJogo()
    {
        if (saveDataPlayerAtual != null)
        {
            saveDataPlayerAtual.SaveGame();
        }
    }

    public SaveData GetPlayer(string name)
    {
        SaveData sd = new SaveData();

        foreach(string x in Directory.GetDirectories("savedata"))
        {
            string f = x.Replace("savedata\\", null);

            if (f == name)
            {
                sd.PlayerName = f;
                sd.LoadGame();
            }
        }

        return sd;
    }

    public SaveData GetPlayer(int id)
    {
        SaveData sd = new SaveData();

        if (NumeroJogadores() < id || NumeroJogadores()==0)
        {
            Debug.LogError("Erro. Tentou encontrar um jogador que nao existe?");
            return null;
        }

        sd.PlayerName = Directory.GetDirectories("savedata")[id].Replace("savedata\\","");

        sd.LoadGame();

        return sd;
    }

    public SaveData GetPlayer(string name, GameMode gameMode)
    {
        SaveData sd = null;

        if (gameMode == null)
        {
            sd = new SaveData();
        }
        else if (gameMode is ModoCarreira)
        {
            sd = new CarreiraSaveData();
        }

        foreach (string x in Directory.GetDirectories("savedata"))
        {
            string f = x.Replace("savedata\\", null);

            if (f == name)
            {
                sd.PlayerName = f;
                sd.LoadGame();
            }
        }

        if (gameMode is ModoCarreira)
        {
            modoAtual = new ModoCarreira();
            ((ModoCarreira)modoAtual).InicializarFromSave((CarreiraSaveData)sd);
        }
        else
        {
            modoAtual = gameMode;
        }

        Debug.Log("Carregou o modo de jogo : " + sd.GetType().Name);

        return sd;
    }

    /// <summary>
    /// Carrega um player para iniciar o jogo
    /// </summary>
    /// <param name="name">nome do jogador</param>
    public void CarregarPlayer(string name, GameMode gameMode)
    {
        Debug.Log("Tentar carregar player " + name);

        nomePlayerAtual = name;

        saveDataPlayerAtual = GetPlayer(name, gameMode);

        if (saveDataPlayerAtual==null)
        {
            Debug.LogError("Erro. Tentou carregar um jogador que nao existe?");
        }
    }

    /// <summary>
    /// Criar um novo jogador
    /// </summary>
    /// <param name="name"></param>
    public void CriarPlayer(string name)
    {
        try
        {
            Directory.CreateDirectory(name);
            Directory.Delete(name);
        }
        catch
        {
            Debug.LogError("Nome invalido");
            return;
        }

        if (Directory.Exists("savedata\\"+name))
        {
            Debug.LogError("Ja existe um jogador com esse nome");
            return;
        }

        Directory.CreateDirectory("savedata\\" + name);

        SaveData game = new SaveData();

        game.PlayerName = name;

        game.SaveGame();

        saveDataPlayerAtual = game;
        nomePlayerAtual = game.PlayerName;

        Debug.Log("player criado!");

        MenuUI();
    }

    public string GetCarroNome(CarroData carro)
    {
        if (carro == CarroData.Vazio)
        {
            return "NENHUM";
        }

        return carros[carro.id].GetComponent<CarroStats>().GetNomeTrim(carro.trimId);
    }

    public string GetCarroNome(CarroPlayerData playerCarro)
    {
        if (playerCarro == CarroPlayerData.Vazio)
        {
            return "NENHUM";
        }

        return carros[playerCarro.id].GetComponent<CarroStats>().GetNomeTrim(playerCarro.trimId);
    }

    public T EncontrarObjeto<T>() where T:MonoBehaviour
    {
        return FindAnyObjectByType<T>();
    }

    public void SairJogo()
    {
        Application.Quit();
    }
}