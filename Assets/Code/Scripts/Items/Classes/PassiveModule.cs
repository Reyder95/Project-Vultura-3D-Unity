using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PassiveModule : Module
{
    private List<int> modifiers = new List<int>();

    public PassiveModule(int id, string name, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, List<int> bonusModifiers, List<int> modifiers, float weight) : base(id, name, VulturaInstance.ItemType.Passive_Module, description,  rarity, icon, bonusModifiers, weight)
    {
        this.modifiers = modifiers;
    }

}
