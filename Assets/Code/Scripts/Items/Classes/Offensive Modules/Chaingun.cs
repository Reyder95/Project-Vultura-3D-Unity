using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaingun : OffenseModule
{
    public Chaingun(VulturaInstance.ItemRarity rarity) : 
    base(
        4,
        "Chaingun",
        "A gun that shoots at a high rate of speed. Each bullet doesn't do a lot of damage, " +
        "but the sheer speed at which this gun shoots is enough to do some serious damage",
        rarity,
        new Texture2D(128, 128),
        new List<int>(),
        true,
        0,
        0,
        0,
        11.15f
    )
    {

    }

    override public void Use()
    {
        Debug.Log("Using chaingun!");
    }
}
