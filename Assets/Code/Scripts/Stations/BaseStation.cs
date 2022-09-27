using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base station class. Each type of station will be derived from this one
public class BaseStation : BaseSelectable
{
    public List<Contact> contacts = new List<Contact>();
    public Contact stationHead;

    public Inventory storage = new Inventory();
    public List<InstantiatedShip> shipStorage = new List<InstantiatedShip>();
    public Market market = new Market();

    public Inventory stockpile = new Inventory();

    public List<Facility> facilities = new List<Facility>();

    // Constructor
    public BaseStation(string faction, string selectableName, string type) : base(faction, selectableName, type)
    {
    }

    // Code that initializes the station and runs each function for initialization.
    public virtual void InitializeStation() 
    {
        int numContacts = Random.Range(2, 5);

        for (int i = 0; i < numContacts; i++)
        {
            contacts.Add(new Contact("Akane Mioka", "SomeFaction", (VulturaInstance.ContactType)Random.Range(1, 4)));
        }

        stationHead = new Contact("Akane Mioka", "SomeFaction", VulturaInstance.ContactType.Station_Head);

        InitializeFacilities();
        InitializeBaseStockpile();
        Debug.Log(stockpile.itemList[0].quantity);
        RunProductionChain();
        Debug.Log(stockpile.itemList[0].quantity);

        foreach (Facility facility in facilities)
            Debug.Log(facility);

        foreach (MarketItem marketItem in market.itemList)
            Debug.Log(marketItem.item);
    }

    public void InitializeFacilities()
    {
        facilities.Add(new LuxuryGoodsFacility());
    }

    public void InitializeBaseStockpile()
    {
        foreach (Facility facility in facilities)
        {
            foreach (FacilityItem consumer in facility.consuming)
            {
                stockpile.Add(new InventoryItem(consumer.itemExec(), Random.Range(25, 50)));
            }
        }
    }

    public void RunProductionChain()
    {
        foreach (Facility facility in facilities)
        {
            List<InventoryItem> producedItems = facility.Produce();

            foreach (InventoryItem producedItem in producedItems)
            {
                market.Add(new MarketItem(producedItem.item, producedItem.quantity, 35, 23));
            }

            stockpile = facility.Consume(stockpile);
        }
    }
}
