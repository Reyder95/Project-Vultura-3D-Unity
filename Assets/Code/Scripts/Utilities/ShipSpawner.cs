using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Takes in a fleet from memory (which everything already instantiated) and spawns it in the world
    public void SpawnFleet(Fleet fleet, Vector3 origin)
    {
        Color32 fleetColor = new Color32(System.Convert.ToByte(Random.Range(0, 256)), System.Convert.ToByte(Random.Range(0, 256)), System.Convert.ToByte(Random.Range(0, 256)), 255);

        // Instantiate fleet commander
        GameObject mainShipPrefab = Instantiate(fleet.FleetCommander.ShipReference, origin, Quaternion.identity);

        mainShipPrefab.GetComponent<PrefabHandler>().SetAssociativeData(fleet.FleetCommander, fleet);
        // Instantiate rest of fleet

        mainShipPrefab.GetComponent<PrefabHandler>().currShip.SetObject(mainShipPrefab);

        mainShipPrefab.GetComponent<PrefabHandler>().SetShipColor(fleetColor);

        if (!fleet.FleetCommander.IsAI)
        {
            mainShipPrefab.GetComponent<PrefabHandler>().InitialPlayer();
            VulturaInstance.currentPlayer = mainShipPrefab;
        }
        


        int radius = 150;

        foreach (InstantiatedShip ship in fleet.FleetShips)
        {
            GameObject shipPrefab = Instantiate(ship.ShipReference, new Vector3(
                Random.Range(origin.x - radius, origin.x + radius),
                Random.Range(origin.y - radius, origin.y + radius),
                Random.Range(origin.z - radius, origin.z + radius)
                ), Quaternion.identity);
            shipPrefab.GetComponent<PrefabHandler>().SetAssociativeData(ship, fleet);
            shipPrefab.GetComponent<PrefabHandler>().currShip.SetObject(shipPrefab);
            shipPrefab.GetComponent<PrefabHandler>().SetShipColor(fleetColor);
        }
    }
}
