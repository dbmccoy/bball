using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGraph {
    GameController map = GameController.Instance;
    List<Hex> nodes;
    public NodeGraph() {
        nodes = map.Hexes;
    }
    public Dictionary<Hex, List<Hex>> edges = new Dictionary<Hex, List<Hex>>();

    public float Cost(Hex a, Hex b) {
        return b.Cost;
    }

    public List<Hex> Neighbors(Hex node) {
        return node.neighbors;//.Where(x => x.traversible).ToList();
    }

}

