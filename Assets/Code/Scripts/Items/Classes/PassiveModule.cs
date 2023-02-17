using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modules that aren't activated, but naturally provide benefits
abstract public class PassiveModule : Module
{

    public PassiveModule(
        int id, 
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
            id, 
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
            galacticPrice
            )
    {
    }

}
