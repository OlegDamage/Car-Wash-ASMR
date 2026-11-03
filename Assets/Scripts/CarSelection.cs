using System;

public static class CarSelection
{
    public static int Choose(int count, int excludedIndex = -1)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "At least one car is required.");
        if (count == 1) return 0;
        if (excludedIndex < 0 || excludedIndex >= count)
            return UnityEngine.Random.Range(0, count);
        int index = UnityEngine.Random.Range(0, count - 1);
        return index >= excludedIndex ? index + 1 : index;
    }
}
