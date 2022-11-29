using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contract
{
    private string destination;
    private Inventory items;
    private string faction;

    public Contract(string destination, Inventory items, string faction)
    {
        this.destination = destination;
        this.items = items;
        this.faction = faction;
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
}
