using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler
{
    private List<ItemStat> main = new List<ItemStat>();
    private List<ItemStat> prefixes = new List<ItemStat>();
    private List<ItemStat> suffixes = new List<ItemStat>();

    public bool AddPrefix(ItemStat itemStat, bool forceAdd)
    {
        if (prefixes.Count < 3 || (prefixes.Count < 4 && forceAdd))
        {
            prefixes.Add(itemStat);
            return true;
        }

        return false;
    }

    public bool AddSuffix(ItemStat itemStat, bool forceAdd)
    {
        if (suffixes.Count < 3 || (prefixes.Count < 4 && forceAdd))
        {
            suffixes.Add(itemStat);
            return true;
        }

        return false;
    }

    public void BuildStatList(VulturaInstance.ItemRarity itemRarity)
    {
        int numStats = 0;

        if (itemRarity == VulturaInstance.ItemRarity.Uncommon)
            numStats = 1;
        else if (itemRarity == VulturaInstance.ItemRarity.Rare)
            numStats = 4;
        else if (itemRarity == VulturaInstance.ItemRarity.Epic)
            numStats = 6;
        else if (itemRarity == VulturaInstance.ItemRarity.Legendary)
            numStats = 7;

        List<StatsData> remainingStats = new List<StatsData>(JSONDataHandler.Stats.data);

        int prefixCount = 0;
        int suffixCount = 0;
        
        int currStats = 0;
        int limitCounter = 0;

        while (currStats < numStats)
        {
            limitCounter++;
            Debug.Log(limitCounter);
            int randInt = Random.Range(0, remainingStats.Count);

            string type = remainingStats[randInt].type;
            
            StatTier chosenTier = GetRandomTier(remainingStats[randInt].tiers);

            int tierValue = Random.Range(chosenTier.values.min, chosenTier.values.max);

            ItemStat newStat = new ItemStat(remainingStats[randInt].key, remainingStats[randInt].effector_key, remainingStats[randInt].modifier, remainingStats[randInt].helper_text, remainingStats[randInt].display_text, tierValue, chosenTier.tierIndex);

            bool force = false;

            if (itemRarity == VulturaInstance.ItemRarity.Legendary)
                force = true;
            
            if (type == "prefix")
            {
                bool success = AddPrefix(newStat, force);

                if (success)
                {
                    currStats++;
                    prefixCount++;
                }
            }
            else if (type == "suffix")
            {
                bool success = AddSuffix(newStat, force);

                if (success)
                {
                    currStats++;
                    suffixCount++;
                }
            }

            Debug.Log(currStats);

        }
    }

    public StatTier GetRandomTier(StatTier[] tiers)
    {
        int randInt = Random.Range(0, tiers.Length);

        return tiers[randInt];
    }

    public List<ItemStat> Main 
    {
        get
        {
            return this.main;
        }
    }

    public List<ItemStat> Prefixes
    {
        get
        {
            return this.prefixes;
        }
    }

    public List<ItemStat> Suffixes
    {
        get
        {
            return this.suffixes;
        }
    }
}
