using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Controlador massivo de tudo o que se passa no projeto, objeto que nunca é apagado e
/// que guarda tudo o que interessa no decorrer do jogo
/// </summary>
public class Controlador : MonoBehaviour
{
    static bool iniciar = true;

    public int dinheiro = 10000;

    public CorridaInfo corridaAtual = null;
    public CorridaRules filtroAtual = null;
    public List<GameObject> currentCarros;

    public GameObject[] carros;
    public GameObject[] aiCarros;

    public List<int> teusCarros = new List<int>();

    public GameObject carNameBox;

    int carroSelecionado = -1;

    public int percentagemJogoGanho = 0;

    public GameMode modoAtual = GameMode.nenhum;

    public static CampeonatosController campeonatos;

    public string nomePlayerAtual="";
    public GameSaveData saveDataPlayerAtual = null;
    
    void Awake()
    {
        //singleton e tudo mais
        if (iniciar)
        {          
            iniciar = false;

            currentCarros = new List<GameObject>();

            //abriu o jogo pela primeira vez!
            if (!Directory.Exists("savedata")|| NumeroJogadores() == 0)
            {
                Directory.CreateDirectory("savedata");
            }

            //encontrar todos os campeonatos do jogo e ordenalos por ID nesta variavel estatica
            campeonatos = new CampeonatosController
            {
                campeonatos = new List<CampSave>()
            };

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
                campeonatos.campeonatos.Add(cs);
                campNum++;
            }
            campeonatos.campeonatos = campeonatos.campeonatos.OrderBy(o => o.id).ToList();

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
                campeonatos = saveDataPlayerAtual.corridas;
                nomePlayerAtual = saveDataPlayerAtual.PlayerName;
                Debug.Log("Carregou o player " + saveDataPlayerAtual.PlayerName);

                int g = 0;

                var cmp = saveDataPlayerAtual.corridas.campeonatos;

                for(int i = 0; i < cmp.Count; i++)
                {
                    var ganhos = cmp[i].ganhos;
                    for(int k = 0; k < ganhos.Count; k++)
                    {
                        if (ganhos[k]) g++;
                    }
                }

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
            if (corridaAtual!=null)
            {
                //posicaofinal deve ser 0 no build
                if (corridaAtual.resultado.posicaoFinal == 0)
                {
                    Debug.Log("Corrida foi cancelada...");
                }
                else
                {
                    //dar o dinheiro relativo ao premio e a nossa posiçao
                    if (corridaAtual.resultado.posicaoFinal != 0)
                    {
                        dinheiro += corridaAtual.premio / corridaAtual.resultado.posicaoFinal;
                        Debug.Log("Dar " + corridaAtual.premio / corridaAtual.resultado.posicaoFinal);
                    }            

                    //se ficamos em 1º lugar!
                    if (corridaAtual.resultado.posicaoFinal == 1)
                    {
                        //adicionar novo e guardar tudo
                        campeonatos.campeonatos[corridaAtual.campeonatoID].ganhos[corridaAtual.corridaID]=true;
                        saveDataPlayerAtual.corridas = campeonatos;
                        saveDataPlayerAtual.dinheiro = dinheiro;
                        saveDataPlayerAtual.SaveGame(saveDataPlayerAtual.PlayerName);
                    }
                }
                currentCarros.Clear();

            }

            MenuUI();
        }
        else
        {
            //se carregou uma pista
            SetupSpawn();
        }
    }

    /// <summary>
    /// Refresh dos elementos atuais do ui
    /// </summary>
    public void MenuUI()
    {
        MenuUI x = FindObjectOfType<MenuUI>();
        if (x!=null)
        {
            x.RefreshUI(this);
        }
        
        CheckProgressoJogo();
    }

    /// <summary>
    /// Atualiza a % de jogo completado
    /// </summary>
    public void CheckProgressoJogo()
    {
        int totalEventos = 0;
        int eventosGanhos = 0;

        foreach (CampSave x in campeonatos.campeonatos)
        {
            totalEventos += x.ganhos.Count;
            eventosGanhos += x.GetGanhos();
        }

        percentagemJogoGanho = (eventosGanhos * 100) / totalEventos;
    }

    /// <summary>
    /// Quando abrimos um nivel que nao o menu, este metodo instancia os veiculo de acordo com o 
    /// corridaAtual, que foi populado ao gerar a corrida no menu
    /// </summary>
    public void SetupSpawn()
    {
        Debug.LogWarning("SETUP SPAWN - "+corridaAtual.startingGrid.Count+" carros");

        GameObject spawn = GameObject.Find("SPAWNS");

        int ind = 0;

        for (ind = 0; ind < corridaAtual.startingGrid.Count; ind++)
        {
            if (corridaAtual.startingGrid[ind] == -1)
            {
                GameObject o = Instantiate(carros[Random.Range(0, carros.Length)]);
                o.transform.position = spawn.transform.GetChild(ind).position;
                o.transform.eulerAngles = spawn.transform.GetChild(ind).localEulerAngles;
                currentCarros.Add(o);
            }
            else
            {
                GameObject o = Instantiate(aiCarros[corridaAtual.startingGrid[ind]]);
                o.transform.position = spawn.transform.GetChild(ind).position;
                o.transform.eulerAngles = spawn.transform.GetChild(ind).localEulerAngles;
                int min = filtroAtual.baseDificuldade - 3;
                if (min < 0) min = 0;
                if (filtroAtual.baseDificuldade!=-1)
                {
                    o.GetComponent<VehicleAIDifficulty>().SetupDificuldade(Random.Range(min,filtroAtual.baseDificuldade));
                }
                currentCarros.Add(o);
            }
            Debug.Log("Colocar um carro novo "+currentCarros.Count());
        }

        PopulateGrelhaPartidaUI();
    }


    /// <summary>
    /// Criar o HUD da grelha de partida no inicio da corrida
    /// </summary>
    public void PopulateGrelhaPartidaUI()
    {
        GameObject grelhaPanel = GameObject.Find("grelhaPartida");

        grelhaPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate {
            StartRace();
        });

        for(int i = 0; i < corridaAtual.startingGrid.Count; i++)
        {
            GameObject go = Instantiate(carNameBox, grelhaPanel.transform);

            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(30f,-30f+(-100f*i));

            go.transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
            if (corridaAtual.startingGrid[i] != -1)
            {
                go.transform.GetChild(1).GetComponent<Text>().text = currentCarros[corridaAtual.startingGrid[i]].GetComponent<CarroStats>().NomeResumido(true);
            }
            else
            {
                go.transform.GetChild(1).GetComponent<Text>().text = "TU";
            }
        }

    }

    /// <summary>
    /// Finalmente lançar os carros e deixa-los andar na pista!
    /// </summary>
    public void StartRace()
    {
        foreach(VehicleAI x in FindObjectsOfType<VehicleAI>())
        {
            x.LancarCarro();
        }

        FindObjectOfType<CarroInputDisable>().LancarCarro();

        GameObject.Find("_VehicleManager").GetComponent<NWH.VehiclePhysics.DesktopInputManager>().vehicleController = GameObject.FindGameObjectWithTag("Vehicle").GetComponent<NWH.VehiclePhysics.VehicleController>();

        GameObject.Find("grelhaPartida").SetActive(false);
    }

    /// <summary>
    /// Gera a corrida final
    /// </summary>
    /// <param name="filtros">Os filtros que controlam os carros que podem participar</param>
    /// <param name="information">Informaçao da corrida</param>
    public void GerarCorrida(CorridaRules filtros, CorridaInfo information)
    {
        corridaAtual = information;
        filtroAtual = filtros;

        int[] gridPossivel = GetCarrosPossiveis(filtros);

        int numCarros = filtros.maxOponentes+1;

        if (numCarros > 0)
        {
            corridaAtual.startingGrid.AddRange(FiltrarPorRaridade2(gridPossivel, numCarros));

            if (corridaAtual.startingGrid.Count == 0)
            {
                Debug.LogError("Nao consegue iniciar a corrida");
                return;
            }

            //Jogador começa em ultimo e tem o ID -1
            corridaAtual.startingGrid[corridaAtual.startingGrid.Count - 1] = -1;
        }
        SceneManager.LoadScene(filtros.nivel);
    }

    /// <summary>
    /// Obter a lista de carros possiveis atraves de regras de filtragem
    /// </summary>
    /// <param name="rules">Classe que contem as regras</param>
    /// <returns></returns>
    public int[] GetCarrosPossiveis(CorridaRules rules)
    {
        List<int> carrosLista = new List<int>();

        for (int i = 0; i < aiCarros.Length; i++)
        {
            CarroStats stats = aiCarros[i].GetComponent<CarroStats>();

            if(rules.maxPeso!=-1&&stats.peso >= rules.maxPeso)
            {
                continue;
            }
            if (rules.maxHP!=-1&&stats.potencia >= rules.maxHP)
            {
                continue;
            }
            if(rules.filtrarTracao&&stats.tracao!=rules.tracao)
            {
                continue;
            }
            carrosLista.Add(i);
        }

        return carrosLista.ToArray();
    }

    /// <summary>
    /// Filtra uma lista de carros por raridade
    /// </summary>
    /// <param name="possiveis">Lista pre gerada dos ids dos carros possiveis</param>
    /// <param name="numCarros">Numero de carros no grid</param>
    /// <returns></returns>
    public int[] FiltrarPorRaridade2(int[] possiveis, int numCarros)
    {
        List<int> final = new List<int>();

        int ce = -1;

        if (possiveis.Length == 0)
        {
            Debug.LogError("Não existem carros possiveis para esta corrida!");
            return final.ToArray();
        }

        while(final.Count < numCarros)
        {
            ce = -1;
            while (ce == -1)
            {
                int rc = possiveis[Random.Range(0, possiveis.Length)];

                CarroStats.Raridade raridade = aiCarros[rc].GetComponent<CarroStats>().raridade;

                if (raridade == CarroStats.Raridade.lendario)
                {
                    if (Random.Range(0, 80) == 24)
                    {
                        ce = rc;
                    }
                }
                else if (raridade == CarroStats.Raridade.especial)
                {
                    if (Random.Range(0, 60) == 24)
                    {
                        ce = rc;
                    }
                }
                else if (raridade == CarroStats.Raridade.raro)
                {
                    if (Random.Range(0, 30) == 24)
                    {
                        ce = rc;
                    }
                }
                else if (raridade == CarroStats.Raridade.incomum)
                {
                    if (Random.Range(0, 15) == 12)
                    {
                        ce = rc;
                        continue;
                    }
                }
                else
                {
                    ce = rc;
                }
            }

            final.Add(ce);
        }

        return final.ToArray();
    }

    /// <summary>
    /// Dar um carro para o jogador ficar com ele
    /// </summary>
    /// <param name="id">id do carro</param>
    /// <param name="metodo">metodo de aquisiçao</param>
    public void DarCarro(int id, CarroStats.MetodoAquisicao metodo)
    {
        if (metodo == CarroStats.MetodoAquisicao.Arcade)
        {
            if (teusCarros.Contains(id))
            {
                Debug.Log("Ja tens este carro, no modo arcade so podes ter 1 carro de cada");
            }
            else
            {
                teusCarros.Add(id);
                Debug.Log("Parabens, possuis agora carro X");
            }
        }
        else if (metodo == CarroStats.MetodoAquisicao.Concessionario)
        {
            //aqui tens que pagar pelo carro :(

            if (dinheiro >= carros[id].GetComponent<CarroStats>().preco)
            {
                dinheiro -= carros[id].GetComponent<CarroStats>().preco;
                teusCarros.Add(id);
                Debug.Log("Parabens, possuis agora carro X");
            }
        }
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

        game.dinheiro = dinheiro;

        game.corridas = campeonatos;

        game.SaveGame(name);

        CarregarJogo();

        Debug.Log("player criado!");
    }

    /// <summary>
    /// Sair do jogo
    /// </summary>
    public void SairJogo()
    {
        Application.Quit();
    }

    public enum GameMode
    {
        carreira,
        arcada,
        livre,
        nenhum
    }
}
