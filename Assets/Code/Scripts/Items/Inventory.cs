using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ExistsStruct {
    public bool exists;
    public int index;
}

public class InventoryItem
{
    public BaseItem item;
    public int quantity;

    public InventoryItem(BaseItem item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class Inventory
{
    public List<InventoryItem> itemList = new List<InventoryItem>();

    public float currCargo = 0;

    public void Add(InventoryItem item)
    {
        ExistsStruct value = ContainsItem(item.item);

        if (value.exists)
        {
            itemList[value.index].quantity += item.quantity;
        }
        else
        {
            itemList.Add(item);
        }

        currCargo += (item.quantity * item.item.Weight);
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

    public void Swap(int idx1, int idx2)
    {
        InventoryItem temp = itemList[idx1];
        itemList[idx1] = itemList[idx2];
        itemList[idx2] = temp;
    }

    public InventoryItem Pop(int index)
    {
        InventoryItem item = itemList[index];
        itemList.RemoveAt(index);
        return item;
    }

    // Primarily used for station stockpiles
    public void ReduceWithoutRemove(int index, int quantity)
    {
        if (itemList.Count > index)
        {
            itemList[index].quantity = itemList[index].quantity - quantity;

            if (itemList[index].quantity < 0)
                itemList[index].quantity = 0;
        }
    }

    public InventoryItem FindItem(BaseItem item)
    {
        foreach (InventoryItem invItem in itemList)
        {
            if (invItem.id == item.id)
                return invItem;
        }

        return null;
    }

    // Debug check the contents of the inventory
    public void PrintContents()
    {
        foreach (InventoryItem item in itemList)
        {
            Debug.Log("Item " + item.item + " Quantity: " + item.quantity);
        }
    }
}
