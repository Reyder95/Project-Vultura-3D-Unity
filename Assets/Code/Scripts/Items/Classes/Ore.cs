using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : UnuseableItem
{
    public Ore(
        string key,
        string name,
        string description,
        Texture2D icon,
        float weight,
        int galacticPrice,
        bool stackable
    ) : base(
        key,
        "Ore",
        name,
        VulturaInstance.ItemType.Ore,
        description,
        VulturaInstance.ItemRarity.Common,
        icon,
        weight,
        galacticPrice,
        stackable
    )
    {
        
    }
}
