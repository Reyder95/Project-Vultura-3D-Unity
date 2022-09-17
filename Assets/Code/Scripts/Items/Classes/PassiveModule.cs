using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PassiveModule : Module
{
    private List<int> modifiers = new List<int>();

    public PassiveModule(int id, string name, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, List<int> bonusModifiers, List<int> modifiers) : base(id, name, VulturaInstance.ItemType.Passive_Module, description,  rarity, icon, bonusModifiers)
    {
        this.modifiers = modifiers;
    }

}
