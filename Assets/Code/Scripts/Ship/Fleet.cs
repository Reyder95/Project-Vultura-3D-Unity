using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet
{
    private System.Guid fleetGUID;
    private InstantiatedShip fleetCommander;
    private List<InstantiatedShip> fleetShips = new List<InstantiatedShip>();
    private string task;
    private string faction;

    public Fleet(System.Guid fleetGUID, string faction, InstantiatedShip fleetCommander, List<InstantiatedShip> fleetShips)
    {
        this.fleetGUID = fleetGUID;
        this.fleetCommander = fleetCommander;
        this.fleetShips = fleetShips;
    }

    public void AddOneShip(InstantiatedShip ship)
    {
        fleetShips.Add(ship);
    }

    // Add a number of ships to the fleet
    public void AddShips(List<InstantiatedShip> ships)
    {
        foreach (InstantiatedShip ship in ships)
        {
            fleetShips.Add(ship);
        }
    }

    // Switches the commander to one that exists in the fleet already. Puts the commander into the fleet list
    public void SwitchCommander(InstantiatedShip newCommander)
    {
        foreach (InstantiatedShip ship in fleetShips)
        {
            if (ship.Equals(newCommander))
            {
                fleetShips.Add(fleetCommander);
                fleetCommander = ship;
            }
        }
    }

    // Splits the fleet. Removes ships from the main fleet and puts them into a second fleet.
    public Fleet SplitFleet(List<InstantiatedShip> ships, InstantiatedShip leader = null)
    {
        List<InstantiatedShip> shipList = new List<InstantiatedShip>();
        InstantiatedShip commander = null;

        foreach (InstantiatedShip ship in ships)
        {
            if (this.fleetCommander == ship)
            {
                Debug.Log("You must switch fleet commanders before assigning them to a new fleet.");
            }
            else if (ShipExists(ship))
            {
                if (leader == ship)
                {
                    commander = leader;
                }

                this.fleetShips.Remove(ship);
                shipList.Add(ship);
            }
        }

        if (commander == null)
        {
            commander = shipList[0];
            shipList.RemoveAt(0);
        }

        return new Fleet(System.Guid.NewGuid(), this.faction, commander, shipList);
    }

    public void SpawnFleetInWorld(bool isPlayerFleet = false)
    {
        Debug.Log("Spawning Commander");

        foreach (InstantiatedShip ship in fleetShips)
        {
            Debug.Log("Spawning " + ship.SelectableName);
        }
    }

    // Checks if a ship exists within the fleet
    public bool ShipExists(InstantiatedShip ship)
    {
        foreach (InstantiatedShip listShip in fleetShips)
        {
            if (ship.Equals(listShip))
            {
                return true;
            }
        }

        return false;
    }

    // -- Properties

    public InstantiatedShip FleetCommander
    {
        get
        {
            return this.fleetCommander;
        }

        set
        {
            this.fleetCommander = value;
        }
    }

    public List<InstantiatedShip> FleetShips
    {
        get
        {
            return this.fleetShips;
        }

        set
        {
            this.fleetShips = value;
        }
    }

    public string Faction
    {
        get
        {
            return this.faction;
        }

        set
        {
            this.faction = value;
        }
    }

    public string Task
    {
        get
        {
            return this.task;
        }

        set
        {
            this.task = value;
        }
    }

    public System.Guid FleetGUID
    {
        get
        {
            return this.fleetGUID;
        }
    }
}
