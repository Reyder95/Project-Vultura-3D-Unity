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

    // Generate an item rarity for an item
    public static VulturaInstance.ItemRarity GenerateItemRarity()
    {
        float randomNum = Random.Range(0, 100);

        if (randomNum < 50)
            return VulturaInstance.ItemRarity.Common;
        else if (randomNum >= 50 && randomNum < 75)
            return VulturaInstance.ItemRarity.Uncommon;
        else if (randomNum >= 75 && randomNum < 90)
            return VulturaInstance.ItemRarity.Rare;
        else if (randomNum >= 90 && randomNum < 97)
            return VulturaInstance.ItemRarity.Epic;
        else
            return VulturaInstance.ItemRarity.Legendary;
    }
}
