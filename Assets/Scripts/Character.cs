using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Character : MonoBehaviour {
    public Hex Hex;
    public float speed;
    public int movePoints;
    public int movePointsRemaining;

    public UnityEvent onStartTurn = new UnityEvent();
    public UnityEvent onCompleteTurn = new UnityEvent();

    Hex moveTarget;
    [HideInInspector]
    public Navigator nav;

    public List<Hex> movePath = new List<Hex>();
    public GameController gc;

    private void Awake() {


    }

    // Start is called before the first frame update
    void Start() {
        Face(Dir.W);
        nav = GetComponent<Navigator>();

        GetHex();

        gc = GameController.Instance;
        gc.players.Add(this);
        gc.onNextTurn.AddListener(NextTurn);
        gc.UnPause.AddListener(ExecuteTurn);

        debugPrefab = Resources.Load("DebugText") as GameObject;
    }

    public void NextTurn() {
        gc.playerOrder.Add(this);
    }

    public void TakeTurn() {
        onStartTurn.Invoke();
    }

    public void ExecuteTurn() {
        nav.ExecutePath();
        if(ActionQueue.Count > 0) {
            action = ActionQueue.Dequeue();
        }
    }

    public void EndTurn() {
        gc.PlayerFinishedTurn(this);
        onCompleteTurn.Invoke();
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

    public bool MoveDir(Dir d) {
        var h = new Move(Hex.Get(d));
        if(h.hex != null) {
            Face(h.hex);
            action = h;
            return true;
        }
        return false;
    }

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
        hex.player = this;
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
        gc.playerOrder.Remove(this);
    }

    public void SetPath(List<Hex> hexes) {
      
        movePath = hexes;
        moveTarget = movePath.FirstOrDefault();
    }

    public void Face(Dir d) {
        transform.forward = Hex.DirV[d];
    }

    public void Face(Hex h) {
        transform.forward = (h.transform.position - transform.position).normalized;
    }

    public Queue<Action> ActionQueue = new Queue<Action>();
    public Action action;

    public List<DebugText> debugs = new List<DebugText>();
    GameObject debugPrefab;

    DebugText currentDebug;

    // Update is called once per frame
    void Update()
    {
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
                //Face(Hex);
                move.isComplete = true;
                EndTurn();
                //Debug.Log("path ok: " + (moveTarget != null).ToString() + " " + movePath.Count + " hexes remaining");
            }
        }
    }
}

public class Action {
    public bool isComplete;
    public int steps = 1;
}

public class Move : Action {

    public Hex hex;

    public Move(Hex h) {
        hex = h;
    }
}

public class PlayerState {
    public Hex hex;

    public PlayerState(Hex h) {
        hex = h;
    }
}