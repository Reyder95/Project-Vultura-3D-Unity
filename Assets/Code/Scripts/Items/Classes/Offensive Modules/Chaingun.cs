using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A chaingun, minigun that shoots very fast
public class Chaingun : OffenseModule
{
    public Chaingun(
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
            "Chaingun",
            name, 
            description, 
            rarity, icon,
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

    override public void Use()
    {
        Debug.Log("Using" + base.Name);
    }
}
