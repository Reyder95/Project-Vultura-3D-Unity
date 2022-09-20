using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : OffenseModule
{
    public RocketLauncher(VulturaInstance.ItemRarity rarity) :
    base(
        5,
        "Rocket Launcher",
        "A launcher that shoots rockets at intense speeds. Can shred the hull of an unsuspecting ship.",
        rarity,
        new Texture2D(128, 128),
        new List<int>(),
        true,
        0,
        0,
        0,
        10.18f
    )
    {

    }

    override public void Use()
    {
        Debug.Log("Using rocket launcher!");
    }
}
