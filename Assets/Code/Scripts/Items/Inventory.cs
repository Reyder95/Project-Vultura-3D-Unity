using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple struct that sends back both a value if the item exists in the inventory, and what index that item exists at
public struct ExistsStruct {
    public bool exists;
    public int index;
}

// An inventory item. Contains the item and a quantity 
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

// An inventory system. Anything with items and quantities should use this. The only exception is market
public class Inventory
{
    public List<InventoryItem> itemList = new List<InventoryItem>();    // The list of items in the inventory

    public float currCargo = 0; // The cargo count. When an item is added or removed, this is modified

    // Add an item to the inventory
    public void Add(InventoryItem item)
    {
        ExistsStruct value = ContainsItem(item.item);
        if (value.exists && item.item.Stackable)
        {
            itemList[value.index].quantity += item.quantity;
        }
        else
        {
            itemList.Add(item);
        }
        currCargo += (item.quantity * item.item.Weight);
        
        EventManager.TriggerEvent("Inventory Modified");
    }

    public void ClearInventory()
    {
        EventManager.TriggerEvent("InventoryModified");
        itemList.Clear();
        currCargo = 0;
    }

    public bool CargoFull(InventoryItem item)
    {
        float theoreticalMaxCargo = item.item.Weight + currCargo;

        if (VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.ShipStats.baseCargo >= theoreticalMaxCargo)
            return false;
        
        return true;
    }

    // Check if an item exists within the inventory
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
            if (itemList[i].item.Key == item.Key)
            {
                value.exists = true;
                value.index = i;
                break;
            }
        }

        return value;
    }

    // Swap two items within the inventory
    public void Swap(int idx1, int idx2)
    {
        EventManager.TriggerEvent("Inventory Modified");
        InventoryItem temp = itemList[idx1];
        itemList[idx1] = itemList[idx2];
        itemList[idx2] = temp;
    }

    // Pop an item fully out of the inventory. Usually used when quantity becomes 0, or need o remove all of the item at once.
    public InventoryItem Pop(int index)
    {
        EventManager.TriggerEvent("Inventory Modified");
        InventoryItem item = itemList[index];
        currCargo -= (item.quantity * item.item.Weight);
        itemList.RemoveAt(index);
        return item;
    }

    // Pop a particular amount of the item, not always the entire thing
    public InventoryItem PopAmount(int index, int quantity)
    {
        if (itemList.Count > index)
        {
            if (itemList[index].quantity >= quantity)
            {
                itemList[index].quantity -= quantity;
                InventoryItem item = new InventoryItem(itemList[index].item, quantity);
                
                if (itemList[index].quantity == 0)
                    itemList.RemoveAt(index);

                currCargo -= (item.quantity * item.item.Weight);

                EventManager.TriggerEvent("Inventory Modified");

                return item;
            }
        }

        return null;
    }

    // Primarily used for station stockpiles
    public void ReduceWithoutRemove(int index, int quantity)
    {
        if (itemList.Count > index)
        {
            currCargo -= (itemList[index].quantity * itemList[index].item.Weight);
            itemList[index].quantity = itemList[index].quantity - quantity;
            

            if (itemList[index].quantity < 0)
            {
                // TODO: Needs testing
                currCargo -= (itemList[index].quantity * itemList[index].item.Weight);
                itemList[index].quantity = 0;
            }
        }

        EventManager.TriggerEvent("Inventory Modified");
    }
    
    // Find an item's index by giving an item
    public int FindItemIndex(BaseItem item)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].item.Key == item.Key)
                return i;
        }

        return -1;
    }

    // Find an item instance in this inventory by giving it an item
    public InventoryItem FindItem(BaseItem item)
    {
        foreach (InventoryItem invItem in itemList)
        {
            if (invItem.item.Key == item.Key)
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
