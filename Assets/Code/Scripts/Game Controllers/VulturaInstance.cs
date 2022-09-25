using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The main game instance of Project Vultura. All main data that needs to be handled and persist among each level will be stored here.
public static class VulturaInstance
{
    public static GameObject currentPlayer;     // A reference to the current player character in the game. Since a player can switch ships, this needs to be kept track of.

    public static List<string> systems = new List<string>();
    public static List<ShipWrapper> ships = new List<ShipWrapper>();

    public static List<BaseSelectable> systemSelectables = new List<BaseSelectable>();
    public static List<BaseSelectable> fleetSelectables = new List<BaseSelectable>();

    public static SelectorList selectorList = new SelectorList();

    public static GameObject fleetList;

    public static void InitializeFleetList()
    {
        fleetList = GameObject.FindWithTag("Fleet List");
    }

    public static void InitializeSystems()
    {
        for (int i = 0; i < 5; i++)
        {
            systems.Add("System" + (i + 1));
        }
    }

    public static float CalculateDistance(GameObject object1, GameObject object2)
    {
        if (object1 != null && object2 != null)
            return Vector3.Distance(object1.transform.position, object2.transform.position) / 100;

        return 0;
    }

    public static void InitializeSelectableObjects()
    {
        GameObject[] systemShips = GameObject.FindGameObjectsWithTag("Ship");

        foreach (GameObject ship in systemShips)
        {
            if (ship.GetComponent<PrefabHandler>().fleetAssociation.FleetCommander == ship.GetComponent<PrefabHandler>().currShip)
                systemSelectables.Add(ship.GetComponent<PrefabHandler>().currShip);
        }

        GameObject[] systemStations = GameObject.FindGameObjectsWithTag("Station");

        foreach (GameObject station in systemStations)
        {
            systemSelectables.Add(station.GetComponent<StationComponent>().station);
        }
    }

    

    public enum ContactType {
        Station_Head,                   // The head of the station. Lets you manage faction diplomacy, or help the station which can increase faction rep
        Bounty_Contractor,              // Has up to 3 bounties you can partake in. Usually involving killing or stealing something.
        Commander,                      // Can be hired to be the captain of a fleet. Each fleet needs at least one commander to function.
        Wandering_Trader,               // Very rare. Can have the best weapons, or best prices of goods. Can be used to play the market to your advantage, or rarely have insane weapons to purchase.
        Crew_Manager,                   // Can be used to crew your ships. Your fleets need crew to maintain
        Station_Administrator,          // Can be hired to run your stations busywork, which means you get a bonus in income from that station.
        Underground_Ship_Dealer,        // Similar to a wandering trader, but just for ships. Ships here are solid and usually better than those from a shipyard. Usually come with some kind of bonus.
        Underground_Weapons_Dealer,     // Similar to a wandering trader, the weapons are not as good but this is more common so easier to gain access to.
        Saboteur                        // Can be used to spy and/or sabotage other factions. Useful to turn the tides of the battlefield if needed, or those who wish to deceive you if they are allies.
    }

    public enum ItemType {
        Trade_Good,
        Ammo,
        Active_Module,
        Passive_Module
    }

    public enum ItemRarity {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public static ItemRarity GenerateItemRarity()
    {
        float randomNum = Random.Range(0, 100);

        if (randomNum < 50)
            return ItemRarity.Common;
        else if (randomNum >= 50 && randomNum < 75)
            return ItemRarity.Uncommon;
        else if (randomNum >= 75 && randomNum < 90)
            return ItemRarity.Rare;
        else if (randomNum >= 90 && randomNum < 97)
            return ItemRarity.Epic;
        else
            return ItemRarity.Legendary;
    }

    public static string enumStringParser(string enumString)
    {
        return enumString.Replace("_", " ");
    }
}
