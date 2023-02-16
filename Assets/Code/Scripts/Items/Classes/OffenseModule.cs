using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Offensive active module. Modules that do damage to the enemy
abstract public class OffenseModule : ActiveModule
{
    // base Damage | range? Might need to go in the Active Module class.. at least the range portion
    private bool usesAmmo;      // Does the module use ammo?
    private int currAmmoType;   // What is the ammo type?
    private int ammoAmount;     // How much ammo is in the module?
    private int possibleAmmoType;   // The possible ammo types that this module can input

    public OffenseModule(int id, string name, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, List<int> bonusModifiers, bool usesAmmo, int currAmmoType, int ammoAmount, int possibleAmmoType, float weight, int galacticPrice) : base(id, name, description, rarity, icon, bonusModifiers, weight, galacticPrice)
    {
        this.usesAmmo = usesAmmo;
        this.currAmmoType = currAmmoType;
        this.ammoAmount = ammoAmount;
        this.possibleAmmoType = possibleAmmoType;
    }

    public bool UsesAmmo
    {
        get
        {
            return this.usesAmmo;
        }
    }

    public int CurrAmmoType
    {
        get
        {
            return this.currAmmoType;
        }
    }

    public int AmmoAmount
    {
        get
        {
            return this.ammoAmount;
        }
    }

    public int PossibleAmmoType
    {
        get
        {
            return this.possibleAmmoType;
        }
    }
}
