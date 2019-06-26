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

    public UnityEvent onNextTurn = new UnityEvent();

    public class OnPossessionChange : UnityEvent<Team> { }
    public OnPossessionChange onPossessionChange = new OnPossessionChange();

    public UnityEvent UnPause = new UnityEvent();
    public UnityEvent NextActionEvent = new UnityEvent();

    public Ball ball;
    public GameObject marker;

    public List<Player> players = new List<Player>();
    public List<Player> playersWithRemainingActions = new List<Player>();

    public Team cpuTeam;
    public Team playerTeam;

    public Team teamWithPossession;

    public List<Hex> Hexes = new List<Hex>();

    public bool isPaused = false;

    public void TogglePause() {
        isPaused = !isPaused;
        if (!isPaused) {
            Debug.Log("unpause");
            UnPause.Invoke();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartGame", .2f);
        marker = (GameObject)Resources.Load("marker");
        onPossessionChange.AddListener(PossessionChange);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) {
            ExecuteTurn();
        }

        if (isPaused) {
            string s = "";

            if (teamWithPossession) {
                s = teamWithPossession.teamName;
            }
            else {
                s = "loose ball";
            }

            string p = isPaused ? "PAUSED" : "ACTION";


            GamePhase.text = "PLANNING: " + s + " " + p;
        }
        else {
            bool readyForNextAction = true;

            foreach (var p in players) {
                if (p.action != null && !p.action.isComplete) {
                    readyForNextAction = false;
                }
            }

            if (readyForNextAction) {
                NextActionEvent.Invoke();
            }

            if(playersWithRemainingActions.Count == 0) {
                NextTurn();
            }
            else {
                //Debug.Log(playersWithRemainingActions.Count);
            }
        }
    }

    public void StartGame() {
        ball = Ball.Instance;
        ball.Shoot(ball.hex.RandomNeighbor(2));
        NextTurn();
    }

    Hex ballLoc;

    public void ExecuteTurn() {
        Ball.Instance.Teleport(ballLoc);
        TogglePause();
    }

    public void NextTurn() {
        Debug.Log("next turn");
        ballLoc = Ball.Instance.hex;
        playersWithRemainingActions = new List<Player>(players);
        TogglePause();
        onNextTurn.Invoke();
    }

    public void PossessionChange(Team t) {
        teamWithPossession = t;
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
