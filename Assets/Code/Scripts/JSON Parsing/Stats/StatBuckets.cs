using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatBuckets
{
    public static Dictionary<string, List<int>> buckets = new Dictionary<string, List<int>>();

    public static void AddStat(string key, StatTier[] tiers)
    {
        List<int> statBucket = new List<int>();

        for (int i = 0; i < tiers.Length; i++)
        {
            for (int j = 0; j < tiers[i].tierIndex; j++)
            {
                statBucket.Add(i);
            }
        }

        buckets.Add(key, statBucket);
    }
}
