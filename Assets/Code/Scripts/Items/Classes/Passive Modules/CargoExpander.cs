using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoExpander : PassiveModule
{
    public CargoExpander(VulturaInstance.ItemRarity rarity) :
    base(
        6,
        "Cargo Expander",
        "A module attached to ships to virtualize additional cargo space. Useful to get squeeze in a little bit more each trip.",
        rarity,
        new Texture2D(128, 128),
        new List<int>(),
        new List<int>()
    )
    {

    }

    override public void Use()
    {
        Debug.Log("Using Cargo Expander");
    }
}
