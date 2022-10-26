using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class UseableItem : BaseItem
{
    // Maybe don't need this?
    public UseableItem(int id, string name, VulturaInstance.ItemType type, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, float weight, int galacticPrice) : base(id, name, type, description, rarity, icon, weight, galacticPrice)
    {

    }

    abstract public void Use();
}
