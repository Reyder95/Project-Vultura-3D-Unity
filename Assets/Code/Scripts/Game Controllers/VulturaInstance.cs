using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The main game instance of Project Vultura. All main data that needs to be handled and persist among each level will be stored here.
public static class VulturaInstance
{
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

        foreach (string system in systems)
        {
            Debug.Log(system);
        }
    }

    public static float CalculateDistance(GameObject object1, GameObject object2)
    {
        return Vector3.Distance(object1.transform.position, object2.transform.position) / 100;
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

    public static GameObject currentPlayer;     // A reference to the current player character in the game. Since a player can switch ships, this needs to be kept track of.

    public enum ContactType {
        Station_Head,
        Bounty_Hunter,
        Commander,
        Wandering_Trader
    }

    public static string enumStringParser(string enumString)
    {
        return enumString.Replace("_", " ");
    }
}
