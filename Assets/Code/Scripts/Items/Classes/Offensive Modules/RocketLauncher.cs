using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rocket launcher gun. Shoots a rocket out
public class RocketLauncher : OffenseModule
{
    public RocketLauncher(
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
