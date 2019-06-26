using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T Random<T>(this List<T> l) {
        int i = UnityEngine.Random.Range(0, l.Count);
        return l[i];
    }
}
