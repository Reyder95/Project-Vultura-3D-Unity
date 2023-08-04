using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The base game controller. Is physically in the game world as a game object, and is under MonoBehaviour.
public class Game : MonoBehaviour
{
    // Singleton instance
    public static Game Instance {get; private set; }

    // Handles stations in the current system.
    public GameObject[] stationPrefabs;     // Station prefab objects in the game world
    public GameObject asteroidFieldPrefab;
    public List<BaseStation> stations = new List<BaseStation>();    // Station C# objects
    public GameObject reticleCanvas;

    public Camera scaledCamera;
    public GameObject currPlanet;

    public GameObject[] shipPrefabs;    // All the possible types of ship prefabs that exist

    public ShipSpawner shipSpawner;     // Handles spawning fleets in the world. Easy for debugging, but will later be used as a way to spawn a fleet of specific ships.

    public GameObject StationUI;        // Handles the station UI pane

    // Debugging Elements
    public GameObject GRAPHY;

    void Start() 
    {
        DontDestroyOnLoad(this.gameObject); // Keeps the game controller loaded at all times

        // Singleton handler
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Load item data
        JSONDataHandler.LoadData();

        ItemManager.InitializeItemBuilder();

        // Generate player ship, then add items to cargo for inventory testing
        ShipStats shipStatsComponent = shipPrefabs[0].GetComponent<PrefabHandler>().GetShipStats();
        InstantiatedShip newShip = ShipFactory.Instance.CreateShip(shipPrefabs[0], "Player Faction", "Player Fleet", "Non AI Fleet", shipStatsComponent, false, new Inventory());
        //newShip.InitializeMounts();
        Fleet playerFleet = new Fleet(System.Guid.NewGuid(), "Player Faction", newShip, new List<InstantiatedShip>());

        // -- Debug -- Add items to inventory
        // TODO: Do with new system
        
        // -- Debug -- Add a second ship to the player fleet
        shipStatsComponent = shipPrefabs[1].GetComponent<PrefabHandler>().GetShipStats();
        newShip = ShipFactory.Instance.CreateShip(shipPrefabs[1], "Player Faction", "Player Fleet", "Non AI Fleet", shipStatsComponent, true, new Inventory());
        //newShip.InitializeMounts();
        playerFleet.AddOneShip(newShip);
        newShip.Fleet = playerFleet;

        // -- Debug -- Spawn the player fleet and additional AI fleets
        shipSpawner.SpawnFleet(playerFleet, new Vector3(0, 0, 0));

        MasterOSManager.Instance.InitializeUI();

        // Load all the main UI elements into the UI Manager
        UI_Manager.LoadUIElements();

        // -- Debug -- Automatically loads conversations for station heads. This kind of stuff will most likely be JSON, and will be changed.
        GlobalConvoHolder.LoadStationHeadConversations();

        //VulturaInstance.InitializeFleetList();
        VulturaInstance.InitializeSystems();    // -- Debug -- Load systems into a global systems array. This will be modified to be a legitimate graphs of stargates and so forth.

        // Finds all the station prefabs (station prefabs are tagged with "Station")
        stationPrefabs = GameObject.FindGameObjectsWithTag("Station");

        // Starts a refresh timer for production of items.
        RefreshTimer.Instance.StartTimer(10f, true);

        // For each station prefab found
        foreach (GameObject station in stationPrefabs)
        {
            // Create a new mining station
            BaseStation newStation = new MiningStation("Station Faction", "Station Name", "Mining Station");
            stations.Add(newStation);
            station.GetComponent<StationComponent>().SetStation(newStation);    // Set this new mining station into each prefab.
            newStation.selectableObject = station;  // Sets the newStation selectable object to the station, so that we can grab the base selectable object

            // -- DEBUG -- Add a ship to station storage just to test and see if it works.
            ShipStats shipStatsStorageComponent = shipPrefabs[1].GetComponent<PrefabHandler>().GetShipStats();  // Grab the stats off of the prefab
            newStation.shipStorage.Add(ShipFactory.Instance.CreateShip(shipPrefabs[1], "Extra ship", "N/A", "Non AI Fleet", shipStatsStorageComponent, false, new Inventory()));
        }

        GenerateInventory();

        GenerateAsteroidField();

        VulturaInstance.currGalaxy = new Galaxy(JSONDataHandler.currGalaxy);
        VulturaInstance.currSystem = VulturaInstance.currGalaxy.systemList[0];
        VulturaInstance.currEntity = VulturaInstance.currSystem.systemEntities[0];
        currPlanet.GetComponent<CurrPlanetHandler>().systemEntity = VulturaInstance.currEntity;

        VulturaInstance.InitializeSelectableObjects();

        shipSpawner.SpawnFleet(FleetGenerator(10), new Vector3(0, 0, 600));
        shipSpawner.SpawnFleet(FleetGenerator(50), new Vector3(0, 0, -600));
        shipSpawner.SpawnFleet(FleetGenerator(15), new Vector3(600, 0, 0));
        shipSpawner.SpawnFleet(FleetGenerator(1000), new Vector3(-600, 0, 0));

        VulturaInstance.AddSelectableToSystem(VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip);

    }

    public void GenerateAsteroidField()
    {
        GameObject afield = Instantiate(asteroidFieldPrefab, VulturaInstance.currentPlayer.transform.position, Quaternion.identity);

        List<string> oreKeys = new List<string>();
        oreKeys.Add("carbon");
        oreKeys.Add("silicon");
        oreKeys.Add("iron");

        afield.GetComponent<AsteroidFieldComponent>().GenerateField(oreKeys);
    }

    public void GenerateInventory()
    {
        int randSize = UnityEngine.Random.Range(3, 10);

        for (int i = 0; i < randSize; i++)
        {
            BaseItem tempItem = ItemManager.GenerateRandomItem();
            int quantity = 1;
            
            if (tempItem.Stackable)
                quantity = UnityEngine.Random.Range(1, 100);

            InventoryItem tempInventoryItem = new InventoryItem(tempItem, quantity);

            VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Add(tempInventoryItem, VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip);
        }
    }

    public void DestroyGameObject(GameObject myObject)
    {
        Destroy(myObject);
    }

    public GameObject InstantiateGameObject(GameObject myObject)
    {
        return Instantiate(myObject, VulturaInstance.currentPlayer.transform.position, VulturaInstance.currentPlayer.transform.rotation);
    }

    // -- Debug -- Show FPS display
    public void ToggleFPS(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        
        if (Module.FpsModuleState == Tayx.Graphy.GraphyManager.ModuleState.OFF)
            Module.FpsModuleState = Tayx.Graphy.GraphyManager.ModuleState.FULL;
        else
            Module.FpsModuleState = Tayx.Graphy.GraphyManager.ModuleState.OFF;
    }

    // -- Debug -- Show RAM display
    public void ToggleRAM(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        
        if (Module.RamModuleState == Tayx.Graphy.GraphyManager.ModuleState.OFF)
            Module.RamModuleState = Tayx.Graphy.GraphyManager.ModuleState.FULL;
        else
            Module.RamModuleState = Tayx.Graphy.GraphyManager.ModuleState.OFF;
    }

    // -- Debug -- Show Audio Display
    public void ToggleAUDIO(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        
        if (Module.AudioModuleState == Tayx.Graphy.GraphyManager.ModuleState.OFF)
            Module.AudioModuleState = Tayx.Graphy.GraphyManager.ModuleState.FULL;
        else
            Module.AudioModuleState = Tayx.Graphy.GraphyManager.ModuleState.OFF;
    }

    // -- Debug -- Show additional / advanced display
    public void ToggleAdvanced(InputAction.CallbackContext context)
    {
        Tayx.Graphy.GraphyManager Module = GRAPHY.GetComponent<Tayx.Graphy.GraphyManager>();
        
        if (Module.AdvancedModuleState == Tayx.Graphy.GraphyManager.ModuleState.OFF)
            Module.AdvancedModuleState = Tayx.Graphy.GraphyManager.ModuleState.FULL;
        else
            Module.AdvancedModuleState = Tayx.Graphy.GraphyManager.ModuleState.OFF;
    }

    // -- Debug -- Enable all GRAPHY displays at once
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
        System.Guid fleetGUID = System.Guid.NewGuid();  // Randomize a fleet ID
        GameObject commanderPrefab = shipPrefabs[UnityEngine.Random.Range(0, shipPrefabs.Length)];  // Randomize a prefab to give to the commander
        ShipStats commanderStats = commanderPrefab.GetComponent<PrefabHandler>().GetShipStats();    // Get the ship stats from the commander's ship prefab
        string faction = "AI Faction " + UnityEngine.Random.Range(0, 100);  // Give the fleet a randomized faction name

        // Create an instantiated ship object for the commander. It creates an official instance of that ship in memory to then be modified and used in the world
        InstantiatedShip commander = ShipFactory.Instance.CreateShip(commanderPrefab, faction.ToString(), "Commander " + UnityEngine.Random.Range(0, 100), "Fleet Commander", commanderStats, true, new Inventory());
        //commander.InitializeMounts();

        List<InstantiatedShip> fleetShips = new List<InstantiatedShip>();   // List of ships that will be within the fleet

        // Create a number of ships, grab their ship stats, add them to the fleet
        for (int i = 0; i < numShips - 1; i++)
        {
            GameObject fleetShipPrefab = shipPrefabs[UnityEngine.Random.Range(0, shipPrefabs.Length)];
            ShipStats fleetShipStats = fleetShipPrefab.GetComponent<PrefabHandler>().GetShipStats();
            fleetShips.Add(ShipFactory.Instance.CreateShip(fleetShipPrefab, faction.ToString(), "Ship # " + (i + 1), "Ship", fleetShipStats, true, new Inventory()));
        }

        Fleet myFleet = new Fleet(fleetGUID, faction.ToString(), commander, fleetShips);

        commander.Fleet = myFleet;

        // Return the created fleet
        return myFleet;
    }
}
