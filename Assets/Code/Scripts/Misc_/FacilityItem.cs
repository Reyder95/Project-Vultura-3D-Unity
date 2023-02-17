using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A facility's item. May be a producer or a consumer
public class FacilityItem
{
    public BaseItem item;
    public int quantity;
    public bool demand;

    public FacilityItem(BaseItem item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
        this.demand = false;
    }
}
