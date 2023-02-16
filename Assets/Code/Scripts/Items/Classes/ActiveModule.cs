using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all active module items. Items that are activated by buttons, rather than passively active
abstract public class ActiveModule : Module
{
    public ActiveModule(int id, string name, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, List<int> bonusModifiers, float weight, int galacticPrice) : base(id, name, VulturaInstance.ItemType.Active_Module, description, rarity, icon, bonusModifiers, weight, galacticPrice)
    {

    }
}
