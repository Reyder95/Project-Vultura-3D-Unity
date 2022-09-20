using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UnuseableItem : BaseItem
{
    // Maybe don't need this?
    public UnuseableItem(int id, string name, VulturaInstance.ItemType type, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, float weight) : base(id, name, type, description, rarity, icon, weight)
    {

    }
}
