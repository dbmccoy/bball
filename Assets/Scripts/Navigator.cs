using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Navigator : MonoBehaviour {
    public Character player;
    public Hex CurrentNode;
    public Hex ActualNode;
    public Hex GoalNode;
    public Hex NextNode;
    public List<Hex> Path;
    public float speed;
    public bool turns;
    public bool trail;
    bool isPlanning = true;

    // Use this for initialization
    void Start() {
        player = GetComponent<Character>();
        //StartCoroutine(EndOfFrame());
    }

    IEnumerator EndOfFrame() {
        yield return new WaitForEndOfFrame();
        SetGoal(GoalNode);
    }

    public GameObject ghost;
    public void CreateGhost() {
        ghost = Instantiate(this.gameObject,transform.position,transform.rotation);
        ghost.GetComponentInChildren<MeshRenderer>().material.color = new Color(.5f, .5f, .5f, .5f);
        ghost.GetComponent<Character>().enabled = false;
    }

    public List<Hex> GhostPath = new List<Hex>();
    public GameObject marker;
    List<GameObject> markers = new List<GameObject>();

    public void AddToGhostPath(List<Hex> hs) {
        if(GhostPath.Count == 0) {
            ActualNode = player.Hex;
            CreateGhost();
        }
        GhostPath.AddRange(hs);
        foreach (var item in hs) {
            var m = Instantiate(GameController.Instance.marker, item.transform.position, Quaternion.identity);
            markers.Add(m);
        }
    }

    public void ClearGhostPath() {
        if (ghost) {
            Destroy(ghost);
            ghost = null;
        }
        GhostPath.Clear();
        markers.ForEach(x => Destroy(x));
    }

    public void ExecutePath() {
        if(GhostPath.Count > 0) {
            player.Teleport(ActualNode);
            player.SetPath(new List<Hex>(GhostPath));
            ClearGhostPath();
        }
    }

    public void SetGoal(Hex goal, bool remove = true) {
        GoalNode = goal;
        CurrentNode = player.Hex;
        Path = CurrentNode.GetPathTo(goal);
        if(Path.Count > 0) {
            if (Path.Count > player.movePointsRemaining) {
                Path = Path.GetRange(0, player.movePointsRemaining);
            }
            Path.ForEach(x => x.Highlight(Color.blue));
            SetNextNode(Path.FirstOrDefault(), remove);
        }
    }

    public Hex NextHex() {
        var n = NextNode;
        SetNextNode(Path.FirstOrDefault());
        return n;
    }

    public void SetNextNode(Hex next, bool remove = true) {
        NextNode = next;
        if (remove) {
            Path.Remove(next);
        }
    }
}