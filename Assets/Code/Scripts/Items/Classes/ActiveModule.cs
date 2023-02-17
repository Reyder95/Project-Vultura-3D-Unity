using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all active module items. Items that are activated by buttons, rather than passively active
abstract public class ActiveModule : Module
{
    public ActiveModule(
        string key, 
        string name, 
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
            key, 
            name, 
            VulturaInstance.ItemType.Active_Module, 
            description, 
            rarity, 
            icon, 
            category_stats, 
            main_stats, 
            boolValues, 
            listValues, 
            overrides, 
            weight, 
            galacticPrice
            )
    {

    }
}
