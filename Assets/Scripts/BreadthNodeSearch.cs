using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BreadthNodeSearch {
    public Dictionary<Hex, Hex> cameFrom
        = new Dictionary<Hex, Hex>();
    public Dictionary<Hex, float> costSoFar
        = new Dictionary<Hex, float>();

    public BreadthNodeSearch(NodeGraph graph, Hex start, Hex goal, bool arrows = false, bool nodes = false, bool lazy = true) {
        var frontier = new PriorityQueue();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0) {
            var current = frontier.Dequeue();

            if (current.Equals(goal) && lazy) {
                //break;
            }

            //for (int i = 0; i < graph.Neighbors(current).Count; i++)
            //{
            //    float newCost = graph.Cost(current, next);
            //}

            foreach (var next in graph.Neighbors(current)) {

                float newCost = graph.Cost(current, next); //costSoFar[current] + 1; //
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    float priority = newCost;
                    frontier.Enqueue(next, newCost);
                    cameFrom[next] = current;
                }
            }
        }
    }
}
