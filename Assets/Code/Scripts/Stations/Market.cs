using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketItem
{
    public BaseItem item;
    public int quantity;
    public int buyPrice;
    public int sellPrice;

    public MarketItem(BaseItem item, int quantity, int buyPrice, int sellPrice)
    {
        this.item = item;
        this.quantity = quantity;
        this.buyPrice = buyPrice;
        this.sellPrice = sellPrice;
    }
}

public class Market
{
    public List<MarketItem> itemList = new List<MarketItem>();

    public float CalculateRelativeDemand(int basePrice, int demand)
    {
        return (float)demand / (basePrice + demand);
    }

    public float CalculateRelativeSupply(int basePrice, int supply)
    {
        return (float)supply / (basePrice + supply);
    }

    public float CalculatePrice(int basePrice, int supply, int demand)
    {
        Debug.Log(CalculateRelativeSupply(basePrice, supply));
        return basePrice * (CalculateRelativeSupply(basePrice, supply) / CalculateRelativeDemand(basePrice, demand));
    }

    public void Add(BaseItem item, int quantity)
    {
        ExistsStruct value = ContainsItem(item);

        if (value.exists)
        {
            itemList[value.index].quantity += quantity;
            itemList[value.index].buyPrice = (int)Mathf.Floor(CalculatePrice(item.GalacticPrice, itemList[value.index].quantity, 5));
        }
        else
        {
            itemList.Add(new MarketItem(item, quantity, item.GalacticPrice, (int)Mathf.Floor(item.GalacticPrice * 0.75f)));
        }
    }

    public InventoryItem Purchase(int index, int quantity)
    {
        BaseItem item;

        if (itemList.Count > index)
        {
            item = itemList[index].item;
            
            if (itemList[index].quantity < quantity)
            {
                return null;
            }

            itemList[index].quantity -= quantity;

            if (itemList[index].quantity == 0)
                itemList.RemoveAt(index);

            return new InventoryItem(item, quantity);
        }
        else
        {
            return null;
        }
    }

    public bool Sell(int index, int quantity)
    {
        if (itemList.Count > index)
        {
            if (itemList[index].quantity < quantity)
            {
                return false;
            }

            itemList[index].quantity = itemList[index].quantity - quantity;

            if (itemList[index].quantity == 0)
                itemList.RemoveAt(index);
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public ExistsStruct ContainsItem(BaseItem item)
    {
        // Create new exists struct value. Defaults are a false exists, and index is 0. Index does not matter if exists is false.
        ExistsStruct value = new ExistsStruct {
            exists = false,
            index = 0
        };

        // Loop through array to find if the value exists. If it does, set the struct and break.
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].item.Id == item.Id)
            {
                value.exists = true;
                value.index = i;
                break;
            }
        }

        return value;
    }
}
