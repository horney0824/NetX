using System.Collections;
using System.Collections.Generic;

public static class ArrayX
{
    public static T[] GrowArray<T>(T[] inArray, int inAddNum)
    {
        var inNum = inArray != null ? inArray.Length : 0;
        var newNum = inNum + inAddNum;

        var newArray = new T[newNum];

        for (int i = 0; i < inNum; ++i)
        {
            newArray[i] = inArray[i];
        }

        return newArray;
    }

    public static T[] FitArray<T>(T[] inArray, int inNewNum)
    {
        var inNum = inArray != null ? inArray.Length : 0;
        var newNum = inNewNum;

        if (newNum == inNum)
        {
            return inArray;
        }

        var newArray = new T[newNum];

        for (int i = 0; i < inNum; ++i)
        {
            newArray[i] = inArray[i];
        }

        return newArray;
    }

    public static T[] RemoveFromArray<T>(T[] inArray, int removeIndex)
    {
        if (inArray != null && removeIndex >= 0 && removeIndex < inArray.Length)
        {
            var newArray = new T[inArray.Length - 1];

            for (int i = 0; i < removeIndex; ++i)
            {
                newArray[i] = inArray[i];
            }

            for (int i = removeIndex + 1; i < inArray.Length; ++i)
            {
                newArray[i - 1] = inArray[i];
            }

            return newArray;
        }

        return inArray;
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, System.Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
}
