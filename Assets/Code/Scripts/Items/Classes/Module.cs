using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all modules.
abstract public class Module : UseableItem
{

    public Module(
        int id, 
        string name, 
        VulturaInstance.ItemType type, 
        string description, 
        VulturaInstance.ItemRarity rarity, 
        Texture2D icon, 
        StatisticValue[] category_stats, 
        StatisticValue[] main_stats, 
        BoolValue[] boolValues, 
        ListValue[] listValues, 
        Override overrides, 
        float weight, 
        int galacticPrice
        ) : base(
            id, 
            name, 
            type, 
            description, 
            rarity, 
            icon, 
            weight, 
            galacticPrice,
            false
            )
    {
    }
}
