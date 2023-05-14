using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    Ore ore;
    int quantity;
    AsteroidSelectable currAsteroid;

    public void SetContent(Ore ore)
    {
        this.ore = ore;

        int quantity = Random.Range(100, 500);

        currAsteroid = new AsteroidSelectable("", "Asteroid", "none");
    }

    public void Mine(InstantiatedShip miningShip)
    {
        int amountMined = Random.Range(2, 5);
        quantity -= amountMined;
        miningShip.Cargo.Add(new InventoryItem(ore, amountMined), miningShip);
        EventManager.TriggerEvent("inventory UI Refresh");
        Debug.Log("Mined!");
    }

    public Ore Ore
    {
        get
        {
            return this.ore;
        }
    }

    public int Quantity
    {
        get
        {
            return this.quantity;
        }
    }

    public AsteroidSelectable CurrAsteroid
    {
        get
        {
            return this.currAsteroid;
        }
    }
}
