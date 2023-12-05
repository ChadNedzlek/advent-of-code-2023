﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ChadNedzlek.AdventOfCode.Library;

public static class Helpers
{
    public static void Deconstruct<T>(this T[] arr, out T a, out T b)
    {
        if (arr.Length != 2)
            throw new ArgumentException($"{nameof(arr)} must be 2 elements in length", nameof(arr));
        a = arr[0];
        b = arr[1];
    }
        
    public static void Deconstruct<T>(this T[] arr, out T a, out T b, out T c)
    {
        if (arr.Length != 3)
            throw new ArgumentException($"{nameof(arr)} must be 2 elements in length", nameof(arr));
        a = arr[0];
        b = arr[1];
        c = arr[2];
    }

    public static IEnumerable<T> AsEnumerable<T>(this T[,] arr)
    {
        for (int i0 = 0; i0 < arr.GetLength(0); i0++)
        for (int i1 = 0; i1 < arr.GetLength(1); i1++)
        {
            yield return arr[i0, i1];
        }
    }

    public static void For<T>(this T[,] arr, Action<T[,], int, int, T> act)
    {
        for (int i0 = 0; i0 < arr.GetLength(0); i0++)
        for (int i1 = 0; i1 < arr.GetLength(1); i1++)
        {
            act(arr, i0, i1, arr[i0,i1]);
        }
    }

    public static void For<T>(this T[,] arr, Action<T[,], int, int> act)
    {
        For(arr, (arr, a, b, __) => act(arr, a, b));
    }

    public static void For<T>(this T[,] arr, Action<int, int> act)
    {
        For(arr, (_, a, b, __) => act(a, b));
    }
        
    public static IEnumerable<T> AsEnumerable<T>(this T value)
    {
        return Enumerable.Repeat(value, 1);
    }

    public static IEnumerable<int> AsEnumerable(this Range range)
    {
        var (start, count) = range.GetOffsetAndLength(int.MaxValue);
        return Enumerable.Range(start, count);
    }

    public static int PosMod(this int x, int q) => (x % q + q) % q;
        
    public static int Gcd(int num1, int num2)
    {
        while (num1 != num2)
        {
            if (num1 > num2)
                num1 = num1 - num2;
 
            if (num2 > num1)
                num2 = num2 - num1;
        }
        return num1;
    }
  
    public static int Lcm(int num1, int num2)
    {
        return (num1 * num2) / Gcd(num1, num2);
    }

    public static void AddOrUpdate<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        TKey key,
        TValue add,
        Func<TValue, TValue> update)
    {
        if (dict.TryGetValue(key, out var existing))
        {
            dict[key] = update(existing);
        }
        else
        {
            dict.Add(key, add);
        }
    }
    public static IImmutableDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(
        this IImmutableDictionary<TKey, TValue> dict,
        TKey key,
        TValue add,
        Func<TValue, TValue> update)
    {
        if (dict.TryGetValue(key, out var existing))
        {
            return dict.SetItem(key, update(existing));
        }
        else
        {
            return dict.Add(key, add);
        }
    }
        
    public static void Increment<TKey>(
        this IDictionary<TKey, int> dict,
        TKey key,
        int amount = 1)
    {
        AddOrUpdate(dict, key, amount, i => i + amount);
    }
        
    public static void Decrement<TKey>(
        this IDictionary<TKey, int> dict,
        TKey key,
        int amount = 1)
    {
        Increment(dict, key, -amount);
    }
        
    public static void Increment<TKey>(
        this IDictionary<TKey, long> dict,
        TKey key,
        long amount = 1)
    {
        AddOrUpdate(dict, key, amount, i => i + amount);
    }

    public static IEnumerable<IEnumerable<T>> Chunks<T>(IEnumerable<T> source, int chunkSize)
    {
        using var enumerator = source.GetEnumerator();
        IEnumerable<T> Inner()
        {
            bool needToRead = false;
            for (int i = 0; i < chunkSize; i++)
            {
                if (needToRead && !enumerator.MoveNext()) yield break;

                yield return enumerator.Current;

                needToRead = true;
            }
        }

        while (enumerator.MoveNext())
        {
            yield return Inner();
        }
    }

    public static bool IncludeVerboseOutput { get; set; }

    public static void Verbose(string text)
    {
        if (IncludeVerboseOutput)
            Console.Write(text);
    }

    public static void VerboseLine(string line)
    {
        if (IncludeVerboseOutput)
            Console.WriteLine(line);
    }

    public static void IfVerbose(Action callback)
    {
        if (IncludeVerboseOutput)
            callback();
    }

    public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
    {
        var i = 0;
        foreach (var item in source)
        {
            if (predicate(item))
                return i;
            i++;
        }

        return -1;
    }

    public static bool TryGet<T>(this IReadOnlyList<IReadOnlyList<T>> input, int i1, int i2, out T value)
    {
        if (i1 < 0 || i1 >= input.Count)
        {
            value = default;
            return false;
        }

        var l = input[i1];
        if (i2 < 0 || i2 >= l.Count)
        {
            value = default;
            return false;
        }

        value = l[i2];
        return true;
    }
        
    public static bool TryGet(this IReadOnlyList<string> input, int i1, int i2, out char value)
    {
        if (i1 < 0 || i1 >= input.Count)
        {
            value = default;
            return false;
        }

        string l = input[i1];
        if (i2 < 0 || i2 >= l.Length)
        {
            value = default;
            return false;
        }

        value = l[i2];
        return true;
    }
}