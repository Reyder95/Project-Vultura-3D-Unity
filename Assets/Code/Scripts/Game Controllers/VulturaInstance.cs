using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The main game instance of Project Vultura. All main data that needs to be handled and persist among each level will be stored here.
public static class VulturaInstance
{
    // -- Enums

    // The types that items can be
    public enum ItemType {
        Trade_Good,
        Ammo,
        Active_Module,
        Passive_Module,
        Ore
    }

    // The rarity of various items
    public enum ItemRarity {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum TransactionLocation {
        SUPPLY,
        DEMAND,
        INVENTORY
    }

    // The type of movement between storage
    public enum MoveType {
        SINGLE,
        ALL,
        SPECIFY
    }

    // The type of response for contacts at stations
    public enum ResponseType {
        Back,
        Shop,
        Basic,
        Commander
    }

    public enum EntityType {
        ASTEROID_FIELD,
        MINING_STATION,
        SHIPYARD,
        RESEARCH_STATION,
        PLANET
    }

    // Different contacts you can see in the station
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

    // Describes the player's status
    public enum PlayerStatus {
        SPACE,      // Roaming the world - actively playing the game
        STATION,    // At a station
        DEAD        // Was blown up. As the kids say "got rekt"
    }

    public static GameObject currentPlayer;     // A reference to the current player character in the game. Since a player can switch ships, this needs to be kept track of.
    public static int playerMoney = 60000;      // The player's starting balance
    public static PlayerStatus playerStatus = PlayerStatus.SPACE;   // The player's current status in the game. May be modified and evolved later on

    public static List<string> systems = new List<string>();    // The list of global systems
    public static List<ShipWrapper> ships = new List<ShipWrapper>();    // The list of ships in memory that will be worked through with AI

    // The different types of selectables in the entity UIs
    public static List<BaseSelectable> systemSelectables = new List<BaseSelectable>();
    public static List<BaseSelectable> fleetSelectables = new List<BaseSelectable>();

    public static List<SelectableEntity> systemEntities = new List<SelectableEntity>();
    public static List<SelectableEntity> subEntities = new List<SelectableEntity>();

    public static SelectorList selectorList = new SelectorList();   // The list of items that are selected by the user

    public static Galaxy currGalaxy;
    public static StarSystem currSystem;
    public static SystemEntity currEntity;

    public static float border = 3000;
    public static SystemEntity currTarget;

    // -- Debug -- The initialized list of systems
    public static void InitializeSystems()
    {
        for (int i = 0; i < 5; i++)
        {
            systems.Add("System" + (i + 1));
        }
    }

    // Calculate the distance between two game objects in the world.
    public static float CalculateDistance(GameObject object1, GameObject object2)
    {
        if (object1 != null && object2 != null)
            return Vector3.Distance(object1.transform.position, object2.transform.position) / 100;

        return 0;
    }

    public static float CalculateCoordinateDistance(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2) * 200;
    }

    // Initialize all the selectabe objects in the system.
    public static void InitializeSelectableObjects()
    {
        systemEntities.Clear();
        foreach (SystemEntity entity in currSystem.systemEntities)
        {
            systemEntities.Add(new SelectableEntity(entity.name, "test", entity, entity.type));

            Game.Instance.reticleCanvas.GetComponent<TestCanvasStuff>().InstantiateReticle(systemEntities.Count - 1);
        }

        // GameObject[] systemShips = GameObject.FindGameObjectsWithTag("Ship");

        // foreach (GameObject ship in systemShips)
        // {
        //     if (ship.GetComponent<PrefabHandler>().fleetAssociation.FleetCommander == ship.GetComponent<PrefabHandler>().currShip)
        //         systemSelectables.Add(ship.GetComponent<PrefabHandler>().currShip);
        // }

        // GameObject[] systemStations = GameObject.FindGameObjectsWithTag("Station");

        // foreach (GameObject station in systemStations)
        // {
        //     systemSelectables.Add(station.GetComponent<StationComponent>().station);
        // }
    }

    public static void RemoveShipsFromEntities()
    {
        int counter = 0;

        while (counter < VulturaInstance.systemEntities.Count)
        {
            if (VulturaInstance.systemEntities[counter].entity.moveableEntity)
                VulturaInstance.systemEntities.RemoveAt(counter);
            else
                counter++;
        }
    }

    public static void RemoveSubEntitiesOfMainEntity(SystemEntity mainEntity)
    {
        int counter = 0;

        while (counter < VulturaInstance.subEntities.Count)
        {
            if (VulturaInstance.subEntities[counter].entity.mainEntity == mainEntity)
                VulturaInstance.subEntities.RemoveAt(counter);
            else
                counter++;
        }
    }

    // Add a selectable item to a system
    public static void AddSelectableToSystem(BaseSelectable item)
    {
        SystemEntity testEntity = new SystemEntity(new Vector3(0, 0, 0), item, item.SelectableName, true, item.Type);
        currSystem.systemEntities.Add(testEntity);
        item.SetEntity(testEntity);

        SelectableEntity selectableEntity = new SelectableEntity(testEntity.name, item.Faction, testEntity, testEntity.type);
        testEntity.SetEntity(selectableEntity);

        if (item.GetType() == typeof(InstantiatedShip))
        {
            Debug.Log((item as InstantiatedShip).Fleet);
            foreach (InstantiatedShip ship in (item as InstantiatedShip).Fleet.FleetShips)
            {
                SystemEntity subEntity = new SystemEntity(new Vector3(0, 0, 0), ship, ship.SelectableName, true, ship.Type);
                ship.SetEntity(subEntity);

                SelectableEntity subSelectableEntity = new SelectableEntity(subEntity.name, ship.Faction, subEntity, subEntity.type);
                subEntity.SetEntity(subSelectableEntity);
            }
        }

        systemEntities.Add(selectableEntity);
        EventManager.TriggerEvent("Selectable Added");
    }

    public static void AddSubSelectableToSystem(BaseSelectable item)
    {
        // SystemEntity testEntity = new SystemEntity(new Vector3(0, 0, 0), item, item.SelectableName, true, item.Type);
        // item.SetEntity(testEntity);

        // SelectableEntity selectableEntity = new SelectableEntity(testEntity.name, item.Faction, testEntity, testEntity.type);
        // testEntity.SetEntity(selectableEntity);

        subEntities.Add(item.entity.aboveEntity);
        EventManager.TriggerEvent("Selectable Added");
    }

    public static void AddSubEntityToSystem(SystemEntity entity)
    {
        SelectableEntity newEntity = new SelectableEntity(entity.name, "test", entity, entity.type);

        subEntities.Add(newEntity);
    }

    // Remove an item from the selectable list
    public static void RemoveFromSystem(BaseSelectable item)
    {
        systemSelectables.Remove(item);
        EventManager.TriggerEvent("Selectable Removed"); 
    }

    // Swap items between different inventories
    public static bool SwapInventory(int index, Inventory invFrom, Inventory invTo, MoveType moveType, bool toStorage, int quantity = 0)
    {
        InstantiatedShip toShip = null;

        if (!toStorage)
            toShip = currentPlayer.GetComponent<PrefabHandler>().currShip;

        InventoryItem inventoryItem = null;
        bool successfulAdd = false;

        if (moveType == MoveType.SINGLE)
        {
            //inventoryItem = invFrom.PopAmount(index, 1);
            inventoryItem = invFrom.ReturnAmountOfItem(index, 1);
        }
        else if (moveType == MoveType.ALL)
        {
            //inventoryItem = invFrom.Pop(index);
            inventoryItem = invFrom.ReturnAllOfItem(index);
        }
        else if (moveType == MoveType.SPECIFY)
        {
            //inventoryItem = invFrom.PopAmount(index, quantity);
            inventoryItem = invFrom.ReturnAmountOfItem(index, quantity);
        }

        if (inventoryItem != null)
        {
            successfulAdd = invTo.Add(inventoryItem, toShip);
            
        }

        if (successfulAdd)
        {
            if (moveType == MoveType.SINGLE)
            {
                inventoryItem = invFrom.PopAmount(index, 1);
            }
            else if (moveType == MoveType.ALL)
            {
                inventoryItem = invFrom.Pop(index);
            }
            else if (moveType == MoveType.SPECIFY)
            {
                inventoryItem = invFrom.PopAmount(index, quantity);
            }

            return true;
        }

        return false;
    }

    public static Color32 GenerateItemColor(ItemRarity rarity)
    {
        if (rarity == ItemRarity.Common)
            return new Color32(128, 128, 128, 255);
        else if (rarity == ItemRarity.Uncommon)
            return new Color32(0, 153, 51, 255);
        else if (rarity == ItemRarity.Rare)
            return new Color32(204, 204, 0, 255);
        else if (rarity == ItemRarity.Epic)
            return new Color32(204, 0, 136, 255);
        else
            return new Color32(255, 128, 0, 255);
    }

    // Parse an enum into a viewable string
    public static string enumStringParser(string enumString)
    {
        return enumString.Replace("_", " ");
    }
}
