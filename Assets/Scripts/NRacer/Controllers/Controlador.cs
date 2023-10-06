using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Assets.Scripts.NRacer.Controllers;

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

    public GameObject[] carros;
    public GameObject[] aiCarros;

    public GameObject carNameBox;

    public CarroData carroSelecionado = CarroData.Vazio;

    public GameMode modoAtual;

    //public static CampeonatosController campeonatos;

    public string nomePlayerAtual="";
    public GameSaveData saveDataPlayerAtual = null;

    public static Controlador instancia;

    public bool mudancasManuais;
    
    void Awake()
    {
        //singleton e tudo mais
        if (iniciar)
        {          
            iniciar = false;
            instancia = this;

            corridaAtual = new CorridaInfo(-1,-1,-1,-1);

            //abriu o jogo pela primeira vez!
            if (!Directory.Exists("savedata")|| NumeroJogadores() == 0)
            {
                Directory.CreateDirectory("savedata");
            }

            //encontrar todos os campeonatos do jogo e ordenalos por ID nesta variavel estatica
            /*campeonatos = new CampeonatosController
            {
                campeonatos = new List<CampSave>()
            };*/

            int campNum = 0;int corrNum = 0;

            foreach(Campeonato x in FindObjectsOfType<Campeonato>())
            {
                CampSave cs = new CampSave();
                cs.id = x.id;
                for(int i = 0; i < x.corridasLista.Length; i++)
                {
                    cs.ganhos.Add(x.corridasLista[i].ganhou);
                    corrNum++;
                }
                //campeonatos.campeonatos.Add(cs);
                campNum++;
            }
            //campeonatos.campeonatos = campeonatos.campeonatos.OrderBy(o => o.id).ToList();

            Debug.Log("Registou " + campNum + " campeonatos. Registou " + corrNum + " corridas.");

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

                int g = 0;

                //TODO: Muito trabalho aqui para ter os saves a funcionar corretamente

                /*var cmp = saveDataPlayerAtual.corridas.campeonatos;

                for(int i = 0; i < cmp.Count; i++)
                {
                    var ganhos = cmp[i].ganhos;
                    for(int k = 0; k < ganhos.Count; k++)
                    {
                        if (ganhos[k]) g++;
                    }
                }*/

                //primeira vez que vai jogar, dar um carro!
                /*if (teusCarros == null || teusCarros.Count == 0)
                {
                    teusCarros = new List<int>();
                    DarCarro(0,0,CarroStats.MetodoAquisicao.ARCADE);
                    saveDataPlayerAtual.carros = teusCarros;
                    saveDataPlayerAtual.SaveGame(nomePlayerAtual);
                }

                SetCarroSelected(0);*/

                Debug.Log("Jogador tem "+saveDataPlayerAtual.carros.Count+" carros.");
                Debug.Log("Jogador tem " + g + " corridas ganhas");
                MenuUI();
            }
        }
        else
        {
            Debug.Log("Ja existe um jogador aberto com nome " + saveDataPlayerAtual.PlayerName);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 0)
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
                        saveDataPlayerAtual.SaveGame(saveDataPlayerAtual.PlayerName);
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

    /// <summary>
    /// Gerar uma simples corrida numa pista aleatoria, com oponentes aleatorios
    /// Usa o carro que tens selecionado
    /// </summary>
    public void GerarCorridaLivre(int pistaId)
    {
        if (carroSelecionado == CarroData.Vazio)
        {
            Debug.LogError("Erro a gerar corrida livre. Nao tens carro selecionado");
            return;
        }

        GerarCorrida(CorridaRules.CorridaLivre(pistaId), new CorridaInfo(0, 2, 0, 0));
    }

    /// <summary>
    /// Obter a lista de carros possiveis atraves de regras de filtragem
    /// </summary>
    /// <param name="rules">Classe que contem as regras</param>
    /// <returns></returns>
    public CarroData[] GetCarrosPossiveis(CorridaRules rules)
    {
        List<CarroData> carrosLista = new List<CarroData>();

        for (int i = 0; i < aiCarros.Length; i++)
        {
            CarroStats stats = aiCarros[i].GetComponent<CarroStats>();

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
                    if (rules.maxPeso != -1 && stats.GetPesoTrim(k) > rules.maxPeso)
                    {
                        continue;
                    }
                    if (rules.maxHP != -1 && stats.GetPotenciaTrim(k) > rules.maxHP)
                    {
                        continue;
                    }
                    if (rules.filtrarTracao && stats.tracao != rules.tracao)
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

                CarroTrim.Raridade raridade = aiCarros[rc.id].GetComponent<CarroStats>().modelo.trimsDisponiveis[rc.trimId].trimRaridade;

                /*if (raridade == CarroTrim.Raridade.LENDARIO)
                {
                    if (Random.Range(0, 40) == 24)
                    {
                        carroEscolhido = rc;
                    }
                }
                else if (raridade == CarroTrim.Raridade.ESPECIAL)
                {
                    if (Random.Range(0, 30) == 24)
                    {
                        carroEscolhido = rc;
                    }
                }
                else if (raridade == CarroTrim.Raridade.RARO)
                {
                    if (Random.Range(0, 15) == 12)
                    {
                        carroEscolhido = rc;
                    }
                }
                else if (raridade == CarroTrim.Raridade.INCOMUM)
                {
                    if (Random.Range(0, 7) == 5)
                    {
                        carroEscolhido = rc;
                    }
                }
                else
                {
                    carroEscolhido = rc;
                }*/

                carroEscolhido = rc;
            }

            final.Add(carroEscolhido);
        }

        return final.ToArray();
    }

    public void SetCarroSelected(int id, int trim)
    {
        carroSelecionado = new CarroData(id, trim);
    }

    /// <summary>
    /// Obter o numero de savegames
    /// </summary>
    /// <returns></returns>
    public int NumeroJogadores()
    {
        return Directory.GetDirectories("savedata").Length;
    }

    public SaveData GetPlayer(string name)
    {
        SaveData sd = new SaveData();

        foreach(string x in Directory.GetDirectories("savedata"))
        {
            string f = x.Replace("savedata\\", null);

            if (f == name)
            {
                sd.LoadGame(f);
            }
        }

        return sd;
    }

    public GameSaveData GetPlayer(int id)
    {
        GameSaveData sd = new GameSaveData();

        if (NumeroJogadores() < id || NumeroJogadores()==0)
        {
            Debug.LogError("Erro. Tentou encontrar um jogador que nao existe?");
            return null;
        }

        sd.LoadGame(Directory.GetDirectories("savedata")[id]);

        return sd;
    }

    public GameSaveData GetPlayer(string name, GameMode gameMode)
    {
        GameSaveData sd =  new GameSaveData();

        foreach (string x in Directory.GetDirectories("savedata"))
        {
            string f = x.Replace("savedata\\", null);

            if (f == name)
            {
                sd.LoadGame(f);
            }
        }

        modoAtual = gameMode;

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
        Directory.CreateDirectory("savedata\\" + name + "\\game");

        GameSaveData game = new GameSaveData();

        //game.dinheiro = dinheiro;

        //game.corridas = campeonatos;

        game.SaveGame(name);

        CarregarJogo();

        Debug.Log("player criado!");
    }

    public string GetCarroNome(CarroData carro)
    {
        if (carro == CarroData.Vazio)
        {
            return "NENHUM";
        }

        return carros[carro.id].GetComponent<CarroStats>().GetNomeTrim(carro.trimId);
    }

    public void SairJogo()
    {
        Application.Quit();
    }
}