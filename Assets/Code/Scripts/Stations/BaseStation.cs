using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

// Base station class. Each type of station will be derived from this one
public class BaseStation : BaseSelectable
{
    public List<Contact> contacts = new List<Contact>();
    public Contact stationHead;

    public Inventory storage = new Inventory();
    public List<InstantiatedShip> shipStorage = new List<InstantiatedShip>();
    public Market market = new Market();

    //public Inventory stockpile = new Inventory();

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
        // foreach (Facility facility in facilities)
        //     Debug.Log(facility);

        // foreach (MarketItem marketItem in market.itemList)
        //     Debug.Log(marketItem.item);

        RunProductionChain();
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
                facility.stockpile.Add(new InventoryItem(consumer.itemExec(), Random.Range(30, 50)));
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
                market.Add(producedItem.item, producedItem.quantity);
            }

            facility.Consume();

            facility.stockpile.PrintContents();
        }
    }
}
