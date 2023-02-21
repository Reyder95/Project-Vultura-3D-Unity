using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modules that aren't activated, but naturally provide benefits
abstract public class PassiveModule : Module
{

    public PassiveModule(
        string key, 
        string category,
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
        int galacticPrice,
        StatHandler statHandler
        ) : base(
            key, 
            category,
            name, 
            VulturaInstance.ItemType.Passive_Module, 
            description,  
            rarity, 
            icon, 
            category_stats, 
            main_stats, 
            boolValues, 
            listValues, 
            overrides,
            weight, 
            galacticPrice,
            statHandler
            )
    {
    }

}
