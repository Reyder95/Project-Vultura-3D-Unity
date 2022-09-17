using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ActiveModule : Module
{
    public ActiveModule(int id, string name, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, List<int> bonusModifiers) : base(id, name, VulturaInstance.ItemType.Active_Module, description, rarity, icon, bonusModifiers)
    {

    }
}
