using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Offensive active module. Modules that do damage to the enemy
abstract public class OffenseModule : ActiveModule
{
    private bool usesAmmo;      // Does the module use ammo?
    private int currAmmoType;   // What is the ammo type?
    private int ammoAmount;     // How much ammo is in the module?
    private int possibleAmmoTypes;   // The possible ammo types that this module can input
    private bool canMine;   // Whether or not this module has the ability to mine. Miners can do additional damage to asteroids

    // Basic stats (on every offense module)
    private int fireRate;   // For beam weapons this will not matter
    private int damage;     // The damage the module does to entities
    private int amountFiredPerShot;



    public OffenseModule(
        string key, 
        string category,
        string name, 
        string description, 
        VulturaInstance.ItemRarity rarity, 
        Texture2D icon, 
        StatisticValue[] category_stats, 
        StatisticValue[] main_stats, 
        BoolValue[] boolValues, 
        ListValue[] listValues, 
        Override overrides, 
        float weight, 
        int galacticPrice
        ) : base(
            key, 
            category,
            name, 
            description, 
            rarity, 
            icon, 
            category_stats, 
            main_stats, 
            boolValues, 
            listValues, 
            overrides, 
            weight, 
            galacticPrice
            )
    {
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

    public int PossibleAmmoTypes
    {
        get
        {
            return this.possibleAmmoTypes;
        }
    }
}
