using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectioExtensions
{
    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
        int count = collection.Count();
        if (count == 0)
        {
            throw new InvalidOperationException("The collection is empty.");
        }
        int index = UnityEngine.Random.Range(0, count);
        return collection.ElementAt(index);
    }

    public static void Shuffle<T>(this IList<T> collection)
    {
        int n = collection.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = collection[k];
            collection[k] = collection[n];
            collection[n] = value;
        }
    }
}