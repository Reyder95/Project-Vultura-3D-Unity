using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class OffenseModule : ActiveModule
{
    // base Damage | range? Might need to go in the Active Module class.. at least the range portion
    private bool usesAmmo;
    private int currAmmoType;
    private int ammoAmount;
    private int possibleAmmoType;

    public OffenseModule(int id, string name, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, List<int> bonusModifiers, bool usesAmmo, int currAmmoType, int ammoAmount, int possibleAmmoType, float weight) : base(id, name, description, rarity, icon, bonusModifiers, weight)
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
