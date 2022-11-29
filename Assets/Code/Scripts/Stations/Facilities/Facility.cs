using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Facility 
{
    public string facilityName;

    public FacilityItem[] producing;
    public FacilityItem[] consuming;

    public Inventory stockpile = new Inventory();

    public bool demand = false;

    public Facility(FacilityItem[] producing, FacilityItem[] consuming, string facilityName)
    {
        this.producing = producing;
        this.consuming = consuming;
        this.facilityName = facilityName;
    }

    public List<InventoryItem> Produce()
    {
        List<InventoryItem> produced = new List<InventoryItem>();
        foreach (FacilityItem item in producing)
        {
            if (!demand)
            {
                produced.Add(new InventoryItem(item.itemExec(), item.quantity));
            }
            else
            {
                produced.Add(new InventoryItem(item.itemExec(), (int)Mathf.Floor(item.quantity / 3)));
            }
        }

        return produced;
    }

    public bool Consume()
    {
        foreach (FacilityItem item in consuming)
        {
            BaseItem execItem = item.itemExec();

            for (int i = 0; i < stockpile.itemList.Count; i++)
            {
                if (stockpile.itemList[i].item.Id == execItem.Id)
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
