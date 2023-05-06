using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionItem
{
    VulturaInstance.TransactionLocation transactionLocation;
    int index;
    bool purchase;
    MarketItem item;

    public TransactionItem(VulturaInstance.TransactionLocation transactionLocation, int index, bool purchase, MarketItem item)
    {
        this.transactionLocation = transactionLocation;
        this.index = index;
        this.purchase = purchase;
        this.item = item;
    }

    public VulturaInstance.TransactionLocation TransactionLocation
    {
        get
        {
            return this.transactionLocation;
        }
    }

    public int Index
    {
        get
        {
            return index;
        }
    }

    public bool Purchase
    {
        get
        {
            return this.purchase;
        }
    }

    public MarketItem Item
    {
        get
        {
            return this.item;
        }
    }
}
