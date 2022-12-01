using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Modules that aren't activated, but naturally provide benefits
abstract public class PassiveModule : Module
{
    private List<int> modifiers = new List<int>();  // The modifiers on this module (the things that this module buffs)

    public PassiveModule(int id, string name, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, List<int> bonusModifiers, List<int> modifiers, float weight, int galacticPrice) : base(id, name, VulturaInstance.ItemType.Passive_Module, description,  rarity, icon, bonusModifiers, weight, galacticPrice)
    {
        this.modifiers = modifiers;
    }

}
