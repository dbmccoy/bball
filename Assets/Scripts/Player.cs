using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    public Hex Hex;
    public float speed;
    public int movePoints;
    public int movePointsRemaining;
    public Team team;

    Ball ball;
    bool isPlanning = true;

    public bool hasBall;

    public Transform ballHolder;

    Hex moveTarget;
    [HideInInspector]
    public Navigator nav;

    public List<Hex> movePath = new List<Hex>();
    public GameController gc;

    private void Awake() {

        
    }

    // Start is called before the first frame update
    void Start()
    {
        Face(Dir.W);
        nav = GetComponent<Navigator>();

        GetHex();

        Ball.Instance.SetHexEvent.AddListener(SenseBallLocation);
        gc = GameController.Instance;
        gc.players.Add(this);
        gc.onNextTurn.AddListener(NextTurn);
        gc.UnPause.AddListener(ExecuteTurn);
        gc.NextActionEvent.AddListener(NextAction);

        team = GetComponentInParent<Team>();
        debugPrefab = Resources.Load("DebugText") as GameObject;
    }

    public void NextTurn() {
        movePointsRemaining = movePoints;
        isPlanning = true;
    }

    public void ExecuteTurn() {
        
        isPlanning = false;
        nav.ExecutePath();
        if(ActionQueue.Count > 0) {
            action = ActionQueue.Dequeue();
        }

    }

    public bool isSelected;

    public void Select() {
        isSelected = true;
    }
    /*
    public void MoveTo(Hex hex) {
        if(movePath.Count == 0) {
            moveTarget = hex;
        }
        Debug.Log("moveto");
        movePath.Add(hex);
    }
    */

    public Hex GetHex() {

        RaycastHit hitInfo;

        if (Physics.Raycast(new Ray(transform.position + transform.up, -transform.up), out hitInfo, 3f, Mouse.Instance.HexMask)) {
            Hex h = hitInfo.transform.GetComponentInParent<Hex>();
            SetHex(h);
            return h;
        }
        else {
            return null;
        }
    }

    public void Teleport(Hex hex) {
        transform.position = hex.transform.position;
        GetHex();
        SetHex(hex);
    }

    public void SetHex(Hex hex) {
        Hex = hex;
        ArriveAt(Hex);
    }

    public void ArriveAt(Hex hex) {
        if(hex.ball != null) {
            TakeBall();
        }
        hex.player = this;
    }

    public void TakeBall() {
        if (hasBall) return;
        hasBall = true;
        ball = Ball.Instance;
        ball.transform.position = new Vector3(ballHolder.position.x,0,ballHolder.position.z);
        ball.transform.SetParent(ballHolder);

        if(gc.teamWithPossession == null || (gc.teamWithPossession != null && gc.teamWithPossession != team)) {
            gc.onPossessionChange.Invoke(team);
        }

        if (isSelected) {
            Mouse.Instance.mode = Mouse.Mode.move;
        }

        Debug.Log("get ball");
    }

    public void NextAction() {
        if(ActionQueue.Count > 0) {
            action = ActionQueue.Dequeue();
        }
        else {
            action = null;
            FinishActions();
        }
    }

    public void FinishActions() {
        ActionQueue.Clear();
        gc.playersWithRemainingActions.Remove(this);
    }

    public void Shoot(Hex h) {
        if (hasBall) {
            if (isPlanning) {
                ActionQueue.Enqueue(new Shoot(h));
            }
            ball.Shoot(h);
        }
    }

    public void Pass(Player p) {
        if (isPlanning) {
            ActionQueue.Enqueue(new Pass(p));
        }
        else {
            if (hasBall) {
                ball.Shoot(p.Hex);
            }
        }
    }

    public void SenseBallLocation(Hex h) {
        if(h == Hex) {
            TakeBall();
        }
    }

    public void SetPath(List<Hex> hexes) {
        if (isPlanning) {
            nav.AddToGhostPath(hexes);
            foreach (var h in hexes) {
                ActionQueue.Enqueue(new Move(h));
            }
            SetHex(hexes.Last());
            transform.position = hexes.Last().transform.position;
        }
        else {
            Debug.Log(movePath.Count);
            movePath = hexes;
            moveTarget = movePath.FirstOrDefault();
        }
        
    }

    public void Face(Dir d) {
        transform.forward = Hex.DirV[d];
    }

    public void Face(Hex h) {
        transform.forward = (transform.position - h.transform.position).normalized;
    }

    public Queue<Action> ActionQueue = new Queue<Action>();
    public Action action;

    public List<DebugText> debugs = new List<DebugText>();
    GameObject debugPrefab;

    DebugText currentDebug;

    // Update is called once per frame
    void Update()
    {
        //planning phase
        int stepsSoFar = 0;

        if (isPlanning) {
            for (int i = 0; i < ActionQueue.Count; i++) {

                var a = ActionQueue.ElementAt(i);
                var v = Vector3.zero;

                if (debugs.Count <= i) {

                    currentDebug = Instantiate(debugPrefab).GetComponent<DebugText>(); ;
                    debugs.Add(currentDebug);

                }
                else {
                    currentDebug = debugs[i];
                }

                if (a is Pass p) {
                    v = p.player.Hex.transform.position;
                }
                if (a is Move m) {
                    v = m.hex.transform.position;
                }
                if (a is Shoot s) {
                    v = s.hex.transform.position;
                }

                //currentDebug.rect.position = currentDebug.worldToUISpace(currentDebug.canvas, v);
                currentDebug.rect.position = Camera.main.WorldToScreenPoint(v);
                stepsSoFar = stepsSoFar + a.steps;
                currentDebug.text.text = stepsSoFar.ToString();
            }
        }
        else {
            foreach (var d in debugs) {
                Destroy(d.gameObject);
            }
            debugs.Clear();
        }
        

        //action phase
        if (gc.isPaused) return;

        if(action is Move move) {
            moveTarget = move.hex;

            if (Vector3.Distance(transform.position, moveTarget.transform.position) > .05) {
                transform.position += (moveTarget.transform.position - transform.position).normalized * Time.deltaTime * speed;
            }
            else if(Hex != moveTarget) {
                Hex = moveTarget;
                ArriveAt(Hex);
                //movePath.Remove(movePath.FirstOrDefault());
                //moveTarget = movePath.FirstOrDefault();
                Face(Hex);
                move.isComplete = true;
                //Debug.Log("path ok: " + (moveTarget != null).ToString() + " " + movePath.Count + " hexes remaining");
            }
        }
        if(action is Pass pass) {
            Pass(pass.player);
            pass.isComplete = true;
        }
        if(action is Shoot shoot) {
            Shoot(shoot.hex);
            shoot.isComplete = true;
        }
    }
}

public class Action {
    public bool isComplete;
    public int steps = 1;
}

public class Shoot : Action {
    public Hex hex;

    public Shoot(Hex h) {
        hex = h;
    }
}

public class Pass : Action {
    public Player player;

    public Pass(Player p) {
        player = p;
    }
}

public class Move : Action {

    public Hex hex;

    public Move(Hex h) {
        hex = h;
    }
}