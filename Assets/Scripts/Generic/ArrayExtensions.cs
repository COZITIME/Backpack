using System.Threading;
using UnityEngine;

/// <summary>
/// Extensions for arrays
/// </summary>
public static class ArrayExtensions
{
    public static bool IsInRange<T>(this T[] array, int index)
    {
        if (array == null)
        {
            return false;
        }

        return index >= 0 && index < array.Length;
    }

    public static bool IsInRange<T>(this T[,] array, Vector2Int index) =>
        IsInRange(array, index.x, index.y);

    public static bool IsInRange<T>(this T[,] array, int x, int y)
    {
        if (array == null)
        {
            return false;
        }

        bool isXInRange = x >= 0 && x < array.GetLength(0);
        bool isYInRange = y >= 0 && y < array.GetLength(1);
        return isXInRange && isYInRange;
    }

    public static bool IsInRange<T>(this T[,,] array, Vector3Int index) =>
        IsInRange(array, index.x, index.y, index.z);

    public static bool IsInRange<T>(this T[,,] array, int x, int y, int z)
    {
        if (array == null)
        {
            return false;
        }

        bool isXInRange = x >= 0 && x < array.GetLength(0);
        bool isYInRange = y >= 0 && y < array.GetLength(1);
        bool isZInRange = z >= 0 && z < array.GetLength(2);
        return isXInRange && isYInRange && isZInRange;
    }

    public static bool TryGetValueInRange<T>(this T[] array, int index, out T value)
    {
        if (!array.IsInRange(index))
        {
            value = default;
            return false;
        }

        value = array[index];
        return true;
    }

    public static bool TryGetValueInRange<T>(this T[,] array, Vector2Int index, out T value) =>
        TryGetValueInRange(array, index.x, index.y, out value);

    public static bool TryGetValueInRange<T>(this T[,] array, int x, int y, out T value)
    {
        if (!array.IsInRange(x, y))
        {
            value = default;
            return false;
        }

        value = array[x, y];
        return true;
    }

    public static bool TryGetValueInRange<T>(this T[,,] array, int x, int y, int z, out T value)
    {
        if (!array.IsInRange(x, y, z))
        {
            value = default;
            return false;
        }

        value = array[x, y, z];
        return true;
    }
}