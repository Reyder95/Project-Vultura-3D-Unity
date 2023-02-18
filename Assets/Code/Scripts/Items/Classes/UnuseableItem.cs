using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An item that doesn't have a "Use" command in the inventory. You cannot use these items. They could be trade goods, or crafting reagents, etc
abstract public class UnuseableItem : BaseItem
{
    // Maybe don't need this?
    public UnuseableItem(
        string key, 
        string category,
        string name, 
        VulturaInstance.ItemType type, 
        string description, 
        VulturaInstance.ItemRarity rarity, 
        Texture2D icon, 
        float weight, 
        int galacticPrice, 
        bool stackable
        ) : base(
            key, 
            category,
            name, 
            type, 
            description, 
            rarity, 
            icon,
            weight, 
            galacticPrice, 
            stackable
            )
    {

    }
}
