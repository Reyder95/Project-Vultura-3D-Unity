using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Randomizer
{
    public static ItemStat GenerateItemStat(StatsData statsData)
    {
        List<int> retrievedBucket = StatBuckets.buckets[statsData.key];
        int randInt = Random.Range(0, retrievedBucket.Count);
        StatTier retrievedTier = statsData.tiers[retrievedBucket[randInt]];
        int value = Random.Range(retrievedTier.values.min, retrievedTier.values.max);

        return new ItemStat(
            statsData.key, 
            statsData.effector_key, 
            statsData.modifier, 
            statsData.helper_text, 
            statsData.display_text, 
            value, 
            retrievedTier.tierIndex
            );
    }
}
