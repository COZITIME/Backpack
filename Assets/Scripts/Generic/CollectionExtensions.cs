using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public static class CollectionExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        // Convert to array for efficient shuffling
        var array = collection.ToArray();
        
        // Fisher-Yates shuffle
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
        }
        
        // Create a new Collection with shuffled elements
        return new Collection<T>(array);
    }

    public static T GetRandomElement<T>(this IEnumerable<T> collection)
    {
        var array = collection.ToArray();
        return array[Random.Range(0, array.Length)];
    }
}