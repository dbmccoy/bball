using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance {
        get {
            if (_instance == null) {
                _instance = Camera.main.GetComponent<GameController>();
            }
            return _instance;
        }
    }

    public Character playerCharPrefab;

    public UnityEvent onEndTurn = new UnityEvent();
    public UnityEvent onNextTurn = new UnityEvent();

    public UnityEvent UnPause = new UnityEvent();

    public GameObject marker;

    public List<Character> players = new List<Character>();
    public List<Character> playerOrder = new List<Character>();

    public List<Hex> Hexes = new List<Hex>();

    public bool isPaused = false;

    public void TogglePause() {
        isPaused = !isPaused;
        if (!isPaused) {
            Debug.Log("unpause");
            UnPause.Invoke();
        }
    }

    Map map;
    Character player;
    Deck deck;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MapGenFinished() {
        map = GameObject.Find("Floor").GetComponent<Map>();
        player = Instantiate(playerCharPrefab) as Character;
        Invoke("StartGame", .2f);
        player.Teleport(map.GetHex(5, 5));
        deck = GameObject.Find("Deck").GetComponent<Deck>();

        marker = (GameObject)Resources.Load("marker");
    }

    public int turn = 0;
    float turnTime;

    // Update is called once per frame
    void Update()
    {
            
    }

    public void PlayerFinishedTurn(Character p) {
        NextPlayer();
    }

    public void NextPlayer() {
        if (playerOrder.Count == 0) {
            EndTurn();
            NextTurn();
        }
        else {
            playerOrder.FirstOrDefault().TakeTurn();
        }
    }

    public void StartGame() {
        NextTurn();
    }

    public void ExecuteTurn() {
        TogglePause();
    }

    public void EndTurn() {
        onEndTurn.Invoke();
    }

    public void NextTurn() {
        
        turn++;
        Debug.Log("starting turn " + turn);
        playerOrder = new List<Character>(players);

        //TogglePause();
        //onNextTurn.Invoke();
        NextPlayer();
    }

    public List<Hex> ReturnPath(Hex start, Hex goal, bool arrows = false, bool nodes = false, bool lazy = true) {
        var search = new BreadthNodeSearch(Graph(), start, goal);  

        Hex current = goal;
        List<Hex> path = new List<Hex>();
        while (current != start) {
            path.Add(current);
            current = search.cameFrom[current];

            try {
                current.CostSoFar = search.costSoFar[current];
            }
            catch {
                //Debug.Log("catch");
                return path;
            }
            //search.cameFrom.Keys.ToList().ForEach(x => Debug.Log("key " + x.name));
            //search.cameFrom.Values.ToList().ForEach(x => Debug.Log("val " + x.name));
            //Debug.Log(search.cameFrom[current].name);

            //current = start;
            //return null;
        }
        path.Reverse();
        if (path == null) {
            Debug.Log("isnull");
        }
        //path.ForEach(x => x.Highlight());
        return path;
    }

    public Text GamePhase;

    NodeGraph graph;

    public NodeGraph Graph() {
        if (graph == null) graph = new NodeGraph();
        return graph;
    }

}
