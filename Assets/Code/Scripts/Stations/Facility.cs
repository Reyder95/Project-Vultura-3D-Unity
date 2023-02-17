using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A station facility that conosumes and produces. The fundamental idea of this will be modified
public class Facility 
{
    public string key;
    public string facilityName;     // The name of the facility

    public List<FacilityItem> producing = new List<FacilityItem>();    // The items the facility produces
    public List<FacilityItem> consuming = new List<FacilityItem>();    // The items the facility consumes

    public Inventory stockpile = new Inventory();   // The current iteme that the station has stocked up. The station sells these.

    public bool demand = false;     // Is this facility in demand?

    public Facility(string key, List<FacilityItem> producing, List<FacilityItem> consuming, string facilityName)
    {
        this.key = key;
        this.producing = producing;
        this.consuming = consuming;
        this.facilityName = facilityName;
    }

    // Produce a set of items
    public List<InventoryItem> Produce()
    {
        List<InventoryItem> produced = new List<InventoryItem>();
        foreach (FacilityItem item in producing)
        {
            if (!demand)
            {
                produced.Add(new InventoryItem(ItemManager.GenerateSpecificBase(item.item.Key), item.quantity));
            }
            else
            {
                produced.Add(new InventoryItem(ItemManager.GenerateSpecificBase(item.item.Key), (int)Mathf.Floor(item.quantity / 3)));
            }
        }

        return produced;
    }

    // Consume a set of items for each item produced
    public bool Consume()
    {
        foreach (FacilityItem item in consuming)
        {

            for (int i = 0; i < stockpile.itemList.Count; i++)
            {
                if (stockpile.itemList[i].item.Key == item.item.Key)
                {
                    stockpile.ReduceWithoutRemove(i, item.quantity);

                    if (!demand)
                    {
                        if(CheckIfDemand(stockpile, i, item))
                        {
                            demand = true;
                            return true;
                        }
                    }
                }

            }
        }

        return false;
    }

    // Check if this station has demand
    public bool CheckIfAnyDemand()
    {
        foreach (FacilityItem consumer in consuming)
        {
            if (consumer.demand)
                return true;
        }

        return false;
    }

    // Check if a particular item is in demand
    public bool CheckIfDemand(Inventory stockpile, int stockpileIndex, FacilityItem item)
    {   
        if (stockpile.itemList[stockpileIndex].quantity <= item.quantity * 3)
            return true;

        return false;
    }
}
