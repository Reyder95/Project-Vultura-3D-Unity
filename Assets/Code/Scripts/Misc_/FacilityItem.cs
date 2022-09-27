using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityItem
{
    public Func<BaseItem> itemExec;
    public int quantity;

    public FacilityItem(Func<BaseItem> itemExec, int quantity)
    {
        this.itemExec = itemExec;
        this.quantity = quantity;
    }
}
