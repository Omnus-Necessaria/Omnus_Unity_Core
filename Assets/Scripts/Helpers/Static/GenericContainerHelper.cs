using System;
using System.Collections.Generic;
using System.Linq;
using SystemRandom = System.Random;
using UnityRandom = UnityEngine.Random;

public static class GenericContainerHelper
{
    #region [List]

    private static readonly SystemRandom RandomGenerator = new SystemRandom();

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = RandomGenerator.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T GetRandomElement<T>(this IList<T> list)
    {
        var index = UnityRandom.Range(0, list.Count);
        return list[index];
    }
    
    public static T GetRandomItem<T>(this T[] array)
    {
        var index = UnityRandom.Range(0, array.Length);
        return array[index];
    }

    #endregion

    #region [Dictionary]

    /// <summary>
    /// Merges any number of dictionaries together.
    /// </summary>
    /// <exception cref="ArgumentException">In case of duplicated keys.</exception>
    /// <returns>Merged result.</returns>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(params Dictionary<TKey, TValue>[] dictionaries)
    {
        return dictionaries.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public static void AddUniq<TKey, TValue>(ref Dictionary<TKey, TValue> dictionary, TKey objectKeyToAdd, TValue objectValueToAdd)
    {
        if (!dictionary.ContainsKey(objectKeyToAdd))
        {
            dictionary.Add(objectKeyToAdd, objectValueToAdd);
        }
    }

    public static void AddUniq<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey objectKeyToAdd, TValue objectValueToAdd)
    {
        if (!dictionary.ContainsKey(objectKeyToAdd))
        {
            dictionary.Add(objectKeyToAdd, objectValueToAdd);
        }
    }

    public static TValue GetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        return dictionary.Values.ToList().GetRandomElement();
    }
    
    public static TKey GetRandomKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
    {
        return dictionary.Keys.ToList().GetRandomElement();
    }

    #endregion
}