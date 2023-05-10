using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles a cargo contract
public class Contract
{
    private string destination;     // The destination of the contract (which system it's in)
    private Inventory items;        // The items that are to be delivered from the contract
    private string faction;         // The faction requesting this to be done
    private int reward;

    public Contract(string destination, Inventory items, string faction, int reward)
    {
        this.destination = destination;
        this.items = items;
        this.faction = faction;
        this.reward = reward;
    }

    public string Destination
    {
        get
        {
            return this.destination;
        }
    }

    public Inventory Items
    {
        get
        {
            return this.items;
        }
    }

    public string Faction
    {
        get
        {
            return this.faction;
        }
    }

    public int Reward
    {
        get
        {
            return this.reward;
        }
    }
}
