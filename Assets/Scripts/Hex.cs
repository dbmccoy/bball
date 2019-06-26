using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hex : MonoBehaviour
{
    MeshRenderer mr;
    GameObject hexObj;
    GameController gc;

    public Ball ball;
    public Player player;

    public float Cost = 1f;

    public enum Flag {
        basket,
        three
    }

    public List<Flag> flags = new List<Flag>();

    // Start is called before the first frame update
    private void Awake() {

        gc = GameController.Instance;

        hexObj = transform.Find("Cylinder").gameObject;
        hexObj.AddComponent(typeof(MeshCollider));
        hexObj.layer = 9;
        gameObject.layer = 9;

        mr = GetComponentInChildren<MeshRenderer>();
        initCol = mr.material.color;

        gc.Hexes.Add(this);
    }

    void Start()
    {
        foreach (var d in System.Enum.GetValues(typeof(Dir))) {
            Get((Dir)d);
        }

        //CachePaths();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate() {
        if (!isHighlight) {
            DeHighlight();
        }
        isHighlight = false;
    }

    Dictionary<Dir, Hex> neighborDict = new Dictionary<Dir, Hex>();
    public List<Hex> neighbors = new List<Hex>();

    public Hex Get(Dir d) {
        Hex hex = null;

        if (!neighborDict.ContainsKey(d)) {
            RaycastHit hitInfo;

            if(Physics.Raycast(new Ray(transform.position, DirV[d]), out hitInfo, .7f,Mouse.Instance.HexMask)) {
                hex = hitInfo.transform.GetComponentInParent<Hex>();
                if(hex != this) {
                    neighborDict[d] = hex;
                    neighbors.Add(hex);
                    Debug.Log(neighbors.Count);
                }
            }
        }

        return hex;
    }

    public Hex RandomNeighbor(int range) {
        List<Hex> hexes = new  List<Hex>(neighbors);
        for (int i = 0; i < range; i++) {
            List<Hex> additions = new List<Hex>();
            foreach (var n in hexes) {
                foreach (var nn in n.neighbors) {
                    if (!hexes.Contains(nn)) {
                        additions.Add(nn);
                    }
                }
            }
            hexes.AddRange(additions);
        }

        return hexes.Random();

    }

    public void MouseOver() {
        foreach (var h in neighbors) {
            //h.Highlight(Color.green);
        }
    }

    bool isHighlight;

    Color initCol;
    public void Highlight(Color c) {
        mr.material.color = c;
        isHighlight = true;
    }

    public void DeHighlight() {
        mr.material.color = initCol;
    }

    public void SetBall(Ball b) {
        b.hex.ball = null;
        b.hex = this;
        ball = b;
    }

    public static Dictionary<Dir, Vector3> DirV = new Dictionary<Dir, Vector3>() {
        { Dir.N, Vector3.forward },
        { Dir.NW, new Vector3(-1,0,1) },
        { Dir.W, new Vector3(-1,0,0) },
        { Dir.SW, new Vector3(-1,0,-1) },
        { Dir.S, new Vector3(0,0,-1) },
        { Dir.SE, new Vector3(1,0,-1) },
        { Dir.E, new Vector3(1,0,0) },
        { Dir.NE, new Vector3(1,0,1) },
    };


    public float CostSoFar;

    Dictionary<Hex, List<Hex>> PathCache = new Dictionary<Hex, List<Hex>>();

    public void CachePaths() {
        PathCache.Clear();

        gc.Hexes.ForEach(x => {
            if (!gc.Hexes.Contains(this)) {
                Debug.Log("hi");
            }
            if (x != this && this.name.ToString() != x.name.ToString()) {
                var p = gc.ReturnPath(this, x);
                PathCache.Add(x, p);
                //Debug.Log(PathCache.Where(i => i.Value != null).ToList().Count);
            }
        });

    }

    public List<Hex> GetPathTo(Hex node) {
        if (!PathCache.ContainsKey(node)) {
            var p = gc.ReturnPath(this, node);
            PathCache.Add(node, p);
        }
        return PathCache[node];
        
    }
}

public enum Dir {
    N,
    NW,
    W,
    SW,
    S,
    SE,
    E,
    NE
}

