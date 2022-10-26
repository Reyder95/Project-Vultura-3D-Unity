using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedShip : BaseSelectable
{
    private int currShield;
    private int currArmor;
    private int currHull;
    private bool isAI;

    private Inventory cargo;

    private GameObject shipReference;

    // Modules of the ship currently installed into this instance. The shipPrefab will contain all the base data for the ship, which
    // we can use and modify to get this specific ship's maximum stats.
    private List<string> activeModules = new List<string>();
    private List<string> passiveModules = new List<string>();

    // The ship stats of the particular item
    public ShipStats shipStats;
    
    public InstantiatedShip(string faction, string selectableName, string type, int shield, int armor, int hull, ShipStats shipStats, bool isAI, GameObject shipReference, Inventory cargo) : base(faction, selectableName, type)
    {
        this.currShield = shield;
        this.currArmor = armor;
        this.currHull = hull;
        this.isAI = isAI;
        this.shipReference = shipReference;
        this.cargo = cargo;

        this.shipStats = shipStats;
    }

    // Adds an active module to the list, stopped by the maximum # of modules for a ship
    public void AddActiveModule(string newActiveModule)
    {
        if (activeModules.Count < 5)
            activeModules.Add(newActiveModule);
    }

    public bool AddToCargo(InventoryItem item)
    {
        float futureCargo = (item.quantity * item.item.Weight) + cargo.currCargo;

        if (futureCargo < shipStats.baseCargo)
        {
            cargo.Add(item);
            return true;
        }

        return false;
    }

    // Adds a passive module to the list, stopped by a maximum # of modules for a ship
    public void AddPassiveModule(string newPassiveModule)
    {
        if (passiveModules.Count < 5)
            passiveModules.Add(newPassiveModule);
    }

    // Removes an active module from the list
    public void RemoveActiveModule(string activeModule)
    {
        activeModules.Remove(activeModule);
    }

    // Removes a passive module from the list
    public void RemovePassiveModule(string passiveModule)
    {
        passiveModules.Remove(passiveModule);
    }

    // -- Functions for getting max stats
    public int GetMaxHealth()
    {
        Debug.Log("Get max health based on modules on top of base ship stats");
        return 0;
    }

    public int GetMaxShields()
    {
        Debug.Log("Get max shields based on modules on top of base ship stats");
        return 0;
    }

    public int GetMaxHull()
    {
        Debug.Log("Get max hull based on modules on top of base ship stats");
        return 0;
    }

    public int GetMaxCargo()
    {
        Debug.Log("Get max cargo based on modules on top of base ship stats");
        return 0;
    }

    // -- Properties

    public int CurrHull
    {
        get
        {
            return this.currHull;
        }
    }

    public int CurrArmor
    {
        get
        {
            return this.currArmor;
        }
    }

    public int CurrShield
    {
        get
        {
            return this.currShield;
        }
    }

    public bool IsAI
    {
        get
        {
            return this.isAI;
        }

        set
        {
            this.isAI = value;
        }
    }

    public List<string> ActiveModules
    {
        get
        {
            return this.activeModules;
        }
    }

    public List<string> PassiveModules
    {
        get
        {
            return this.passiveModules;
        }
    }

    public ShipStats ShipStats
    {
        get
        {
            return this.shipStats;
        }
    }

    public GameObject ShipReference
    {
        get
        {
            return this.shipReference;
        }
    }

    public Inventory Cargo
    {
        get
        {
            return this.cargo;
        }
    }
}
