using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Facility 
{
    public string facilityName;
    public bool demand;

    public FacilityItem[] producing;
    public FacilityItem[] consuming;

    public Facility(FacilityItem[] producing, FacilityItem[] consuming)
    {
        Debug.Log(producing[0].itemExec);
        this.producing = producing;
        this.consuming = consuming;
    }

    public List<InventoryItem> Produce()
    {
        List<InventoryItem> produced = new List<InventoryItem>();
        foreach (FacilityItem item in producing)
        {
            Debug.Log(item.itemExec);
            produced.Add(new InventoryItem(item.itemExec(), item.quantity));
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
                    return stockpile;
                }

            }
        }
        return null;
    }
}
