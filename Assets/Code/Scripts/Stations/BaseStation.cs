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

    public List<Contract> contracts = new List<Contract>();

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
        stationHead.LoadConversation();

        int randomContractCount = Random.Range(2, 6);

        for (int i = 0; i < randomContractCount; i++)
        {
            Inventory contractInventory = new Inventory();
            Category category = JSONDataHandler.FindCategoryByKey("trade_good");
            List<ItemData> tradeGoods = JSONDataHandler.FindBasesByCategory(category);
            int itemCount = Random.Range(1, tradeGoods.Count);


            for (int j = 0; j < itemCount; j++)
            {
                int randItem = Random.Range(0, tradeGoods.Count);
                BaseItem contractItem = ItemManager.GenerateItem(category, tradeGoods[randItem]);
                tradeGoods.RemoveAt(randItem);
                int randQuantity = Random.Range(1, 400);
                contractInventory.Add(new InventoryItem(contractItem, randQuantity), null);
            }
            
            this.AddContract(contractInventory, "system2", "some-faction", Random.Range(1000, 50000));
        }

        InitializeFacilities();
        InitializeBaseStockpile();

        RunProductionChain();
    }
    public void InitializeFacilities()
    {
        Facility newFacility = ItemManager.GenerateFacility("cookery");
        facilities.Add(newFacility);
    }

    public void InitializeBaseStockpile()
    {
        foreach (Facility facility in facilities)
        {
            foreach (FacilityItem consumer in facility.consuming)
            {
                facility.stockpile.Add(new InventoryItem(ItemManager.GenerateSpecificBase(consumer.item.Key), Random.Range(30, 50)), null);
            }
        }
    }

    public void AddContract(Inventory contractInventory, string destination, string faction, int reward)
    {
        this.contracts.Add(new Contract(destination, contractInventory, faction, reward));
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

            bool inDemand = facility.Consume();

            if (inDemand)
            {
                foreach (FacilityItem item in facility.consuming)
                {
                    market.AddDemandSeller(ItemManager.GenerateSpecificBase(item.item.Key));
                }
            }
            

            facility.stockpile.PrintContents();
        }
    }

    public bool InsertIntoStockpile(BaseItem item, int quantity)
    {
        foreach (Facility facility in facilities)
        {
            if (facility.demand)
            {
                foreach (FacilityItem consumer in facility.consuming)
                {
                    BaseItem consumerItem = ItemManager.GenerateSpecificBase(consumer.item.Key);

                    if (consumerItem.Key == item.Key)
                    {
                        facility.stockpile.Add(new InventoryItem(item, quantity), null);
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
