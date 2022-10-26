using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    [Serializable]
    public struct ItemPairing
    {
        public int id;
        public BaseItem item;
    }

    public static Game Instance {get; private set; }


    public ItemPairing[] items;

    // Handles all the station prefabs in the level
    public GameObject[] stationPrefabs;

    public List<BaseStation> stations = new List<BaseStation>();

    public GameObject[] shipPrefabs;

    public ShipSpawner shipSpawner;

    // UI Elements
    public GameObject entityList;

    // Debugging Elements
    public GameObject GRAPHY;
    

    //int prefabCount = 0;

    void Start() 
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        VulturaInstance.InitializeFleetList();
        VulturaInstance.InitializeSystems();

        // Finds all the station prefabs (station prefabs are tagged with "Station")
        stationPrefabs = GameObject.FindGameObjectsWithTag("Station");

        RefreshTimer.Instance.StartTimer(10f, true);

        // For each station prefab found
        foreach (GameObject station in stationPrefabs)
        {
            float randValue = UnityEngine.Random.Range(-10.0f, 10.0f);
            // Create a new mining station
            BaseStation newStation = new MiningStation("Station Faction", "Station Name", "Mining Station");
            stations.Add(newStation);

            station.GetComponent<StationComponent>().SetStation(newStation);    // Set this new mining station into each prefab.
            newStation.selectableObject = station;
            ShipStats shipStatsStorageComponent = shipPrefabs[1].GetComponent<PrefabHandler>().GetShipStats();
            newStation.shipStorage.Add(new InstantiatedShip("Extra ship", "N/A", "Non AI Fleet", shipStatsStorageComponent.baseHealth, shipStatsStorageComponent.baseArmor, shipStatsStorageComponent.baseHull, shipStatsStorageComponent, false, shipPrefabs[1], new Inventory()));
        }

        //Generate player ship
        ShipStats shipStatsComponent = shipPrefabs[0].GetComponent<PrefabHandler>().GetShipStats();
        InstantiatedShip newShip = new InstantiatedShip("Player Faction", "Player Fleet", "Non AI Fleet", shipStatsComponent.baseHealth, shipStatsComponent.baseArmor, shipStatsComponent.baseHull, shipStatsComponent, false, shipPrefabs[0], new Inventory());
        Fleet playerFleet = new Fleet(System.Guid.NewGuid(), "Player Faction", newShip, new List<InstantiatedShip>());
        newShip.AddToCargo(new InventoryItem(ItemFactoryComponent.Instance.ItemFactoryDict[3].Create(), 10));
        newShip.AddToCargo(new InventoryItem(ItemFactoryComponent.Instance.ItemFactoryDict[6].Create(), 2));
        newShip.AddToCargo(new InventoryItem(ItemFactoryComponent.Instance.ItemFactoryDict[1].Create(), 25));
        newShip.AddToCargo(new InventoryItem(ItemFactoryComponent.Instance.ItemFactoryDict[2].Create(), 25));
        
        shipStatsComponent = shipPrefabs[1].GetComponent<PrefabHandler>().GetShipStats();
        newShip = new InstantiatedShip("Player Faction", "Player Fleet", "Non AI Fleet", shipStatsComponent.baseHealth, shipStatsComponent.baseArmor, shipStatsComponent.baseHull, shipStatsComponent, true, shipPrefabs[1], new Inventory());
        playerFleet.AddOneShip(newShip);

        shipSpawner.SpawnFleet(playerFleet, new Vector3(0, 0, 0));

        shipSpawner.SpawnFleet(FleetGenerator(10), new Vector3(0, 0, 600));

        shipSpawner.SpawnFleet(FleetGenerator(50), new Vector3(0, 0, -600));

        shipSpawner.SpawnFleet(FleetGenerator(15), new Vector3(600, 0, 0));
        shipSpawner.SpawnFleet(FleetGenerator(1000), new Vector3(-600, 0, 0));

        VulturaInstance.InitializeSelectableObjects();
        //entityList.GetComponent<EntityList>().DisplayItems(VulturaInstance.systemSelectables);
    }

    public void ToggleFPS(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        
        if (Module.FpsModuleState == Tayx.Graphy.GraphyManager.ModuleState.OFF)
            Module.FpsModuleState = Tayx.Graphy.GraphyManager.ModuleState.FULL;
        else
            Module.FpsModuleState = Tayx.Graphy.GraphyManager.ModuleState.OFF;
    }

    public void ToggleRAM(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        
        if (Module.RamModuleState == Tayx.Graphy.GraphyManager.ModuleState.OFF)
            Module.RamModuleState = Tayx.Graphy.GraphyManager.ModuleState.FULL;
        else
            Module.RamModuleState = Tayx.Graphy.GraphyManager.ModuleState.OFF;
    }

    public void ToggleAUDIO(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        
        if (Module.AudioModuleState == Tayx.Graphy.GraphyManager.ModuleState.OFF)
            Module.AudioModuleState = Tayx.Graphy.GraphyManager.ModuleState.FULL;
        else
            Module.AudioModuleState = Tayx.Graphy.GraphyManager.ModuleState.OFF;
    }

    public void ToggleAdvanced(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        
        if (Module.AdvancedModuleState == Tayx.Graphy.GraphyManager.ModuleState.OFF)
            Module.AdvancedModuleState = Tayx.Graphy.GraphyManager.ModuleState.FULL;
        else
            Module.AdvancedModuleState = Tayx.Graphy.GraphyManager.ModuleState.OFF;
    }

    public void EnableAll(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        Tayx.Graphy.GraphyManager.ModuleState off = Tayx.Graphy.GraphyManager.ModuleState.OFF;
        Tayx.Graphy.GraphyManager.ModuleState on = Tayx.Graphy.GraphyManager.ModuleState.FULL;

        if (Module.FpsModuleState == off || Module.RamModuleState == off || Module.AudioModuleState == off || Module.AdvancedModuleState == off)
        {
            Module.FpsModuleState = on;
            Module.RamModuleState = on;
            Module.AudioModuleState = on;
            Module.AdvancedModuleState = on;
        }
        else
        {
            Module.FpsModuleState = off;
            Module.RamModuleState = off;
            Module.AudioModuleState = off;
            Module.AdvancedModuleState = off;
        }
    }

    // Generates a numShip number of ships. Fleet commander included
    private Fleet FleetGenerator(int numShips)
    {
        System.Guid fleetGUID = System.Guid.NewGuid();
        GameObject commanderPrefab = shipPrefabs[UnityEngine.Random.Range(0, shipPrefabs.Length)];
        ShipStats commanderStats = commanderPrefab.GetComponent<PrefabHandler>().GetShipStats();
        string faction = "AI Faction " + UnityEngine.Random.Range(0, 100);

        InstantiatedShip commander = new InstantiatedShip(faction.ToString(), "Commander " + UnityEngine.Random.Range(0, 100), "Fleet Commander", commanderStats.baseHealth, commanderStats.baseArmor, commanderStats.baseHull, commanderStats, true, commanderPrefab, new Inventory());

        List<InstantiatedShip> fleetShips = new List<InstantiatedShip>();

        for (int i = 0; i < numShips - 1; i++)
        {
            GameObject fleetShipPrefab = shipPrefabs[UnityEngine.Random.Range(0, shipPrefabs.Length)];
            ShipStats fleetShipStats = fleetShipPrefab.GetComponent<PrefabHandler>().GetShipStats();
            fleetShips.Add(new InstantiatedShip(faction.ToString(), "Ship # " + (i + 1), "Ship", fleetShipStats.baseHealth, fleetShipStats.baseArmor, fleetShipStats.baseHull, fleetShipStats, true, fleetShipPrefab, new Inventory()));
        }

        return new Fleet(fleetGUID, faction.ToString(), commander, fleetShips);
    }

    // private void GenerateAIShips(string system_name)
    // {
    //     List<ShipWrapper> removedShips = new List<ShipWrapper>();

    //     foreach (ShipWrapper ship in VulturaInstance.ships)
    //     {
    //         if (ship.system == system_name)
    //         {
    //             GameObject myShipPrefab = Instantiate(ship.aiShip.instantiatedShip.shipPrefab, new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), Random.Range(-200, 200)), Quaternion.identity);

    //             myShipPrefab.GetComponent<PrefabHandler>().currShip = ship.aiShip;
    //             myShipPrefab.GetComponent<PrefabHandler>().currShip.SetObject(myShipPrefab);
    //             //myShipPrefab.GetComponent<PrefabHandler>().InitializePrefab();

    //             removedShips.Add(ship);
    //         }
    //     }

    //     foreach(ShipWrapper ship in removedShips)
    //     {
    //         VulturaInstance.ships.Remove(ship);
    //     }
    // }
}
