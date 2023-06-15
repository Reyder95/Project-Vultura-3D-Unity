using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFactory : MonoBehaviour
{
    public static ShipFactory Instance {get; private set;}

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
    }

    public InstantiatedShip CreateShip(GameObject shipReference, string faction, string selectableName, string type, ShipStats shipStats, bool isAI, Inventory cargo)
    {
        InstantiatedShip newShip = new InstantiatedShip(faction, selectableName, type, shipStats.baseHealth, shipStats.baseArmor, shipStats.baseHull, shipStats, isAI, shipReference, cargo);

        return newShip;
    }
}
