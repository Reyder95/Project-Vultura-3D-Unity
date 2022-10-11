using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Facility 
{
    public string facilityName;

    public FacilityItem[] producing;
    public FacilityItem[] consuming;

    public Facility(FacilityItem[] producing, FacilityItem[] consuming)
    {
        this.producing = producing;
        this.consuming = consuming;
    }

    public List<InventoryItem> Produce()
    {
        List<InventoryItem> produced = new List<InventoryItem>();
        foreach (FacilityItem item in producing)
        {
            if (!CheckIfAnyDemand())
            {
                produced.Add(new InventoryItem(item.itemExec(), item.quantity));
                Debug.Log("Produced " + item.quantity.ToString());
            }
            else
            {
                produced.Add(new InventoryItem(item.itemExec(), (int)Mathf.Floor(item.quantity / 3)));
                Debug.Log("Produced " + Mathf.Floor(item.quantity / 3).ToString() + " instead of " + item.quantity.ToString());
            }
        }

        return produced;
    }

    public Inventory Consume(Inventory stockpile)
    {
        foreach (FacilityItem item in consuming)
        {
            BaseItem execItem = item.itemExec();

            for (int i = 0; i < stockpile.itemList.Count; i++)
            {
                if (stockpile.itemList[i].item.Id == execItem.Id)
                {
                    stockpile.ReduceWithoutRemove(i, item.quantity);
                    item.demand = CheckIfDemand(stockpile, i, item);
                    return stockpile;
                }

            }
        }
        return null;
    }

    public bool CheckIfAnyDemand()
    {
        foreach (FacilityItem consumer in consuming)
        {
            if (consumer.demand)
                return true;
        }

        return false;
    }

    public bool CheckIfDemand(Inventory stockpile, int stockpileIndex, FacilityItem item)
    {   
        if (stockpile.itemList[stockpileIndex].quantity <= item.quantity * 3)
            return true;

        return false;
    }
}
