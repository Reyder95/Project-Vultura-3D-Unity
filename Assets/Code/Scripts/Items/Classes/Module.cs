using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Module : UseableItem
{
    private List<int> bonusModifiers = new List<int>();

    public Module(int id, string name, VulturaInstance.ItemType type, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, List<int> bonusModifiers, float weight) : base(id, name, type, description, rarity, icon, weight)
    {
        this.bonusModifiers = bonusModifiers;
    }
}
