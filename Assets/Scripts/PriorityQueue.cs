using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue {
    // I'm using an unsorted array for this example, but ideally this
    // would be a binary heap. There's an open issue for adding a binary
    // heap to the standard C# library: https://github.com/dotnet/corefx/issues/574
    //
    // Until then, find a binary heap class:
    // * https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    // * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
    // * http://xfleury.github.io/graphsearch.html
    // * http://stackoverflow.com/questions/102398/priority-queue-in-net


    private List<NodeFloatTuple> elements = new List<NodeFloatTuple>();

    public int Count {
        get { return elements.Count; }
    }

    public void Enqueue(Hex item, float priority) {
        elements.Add(new NodeFloatTuple(item, priority));
    }

    public Hex Dequeue() {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++) {
            if (elements[i].f < elements[bestIndex].f) {
                bestIndex = i;
            }
        }

        Hex bestItem = elements[bestIndex].n;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

public class NodeFloatTuple {
    public Hex n;
    public float f;
    public NodeFloatTuple(Hex _n, float _f) {
        n = _n;
        f = _f;
    }
}
