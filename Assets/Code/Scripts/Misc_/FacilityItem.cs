using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A facility's item. May be a producer or a consumer
public class FacilityItem
{
    public Func<BaseItem> itemExec;
    public int quantity;
    public bool demand;

    public FacilityItem(Func<BaseItem> itemExec, int quantity)
    {
        this.itemExec = itemExec;
        this.quantity = quantity;
        this.demand = false;
    }
}
