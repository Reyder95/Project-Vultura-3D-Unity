using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An item with a "use" command in the inventory. You can use these items to equip, as health potion-type items, etc
abstract public class UseableItem : BaseItem
{
    // Maybe don't need this?
    public UseableItem(int id, string name, VulturaInstance.ItemType type, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, float weight, int galacticPrice) : base(id, name, type, description, rarity, icon, weight, galacticPrice)
    {

    }

    // Base function of "Use" that dictates how this item will be used.
    abstract public void Use();
}
