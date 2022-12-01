using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The base item class that all items derive from
abstract public class BaseItem
{
    [Header("Item Information")]
    [SerializeField] int id;    // The identification of the item itself
    [SerializeField] string name;   // The name of the item
    [SerializeField] float weight;  // The weight of the item itself, for storage purposes
    [SerializeField] private VulturaInstance.ItemType type;     // The type of the item
    [SerializeField] private string description;            // The description that is shown in the tooltip of an item
    [SerializeField] private VulturaInstance.ItemRarity rarity;     // The rarity of the item, changing the look of the tooltip and the color
    [SerializeField] private int galacticPrice;         // The base price of the item, changes how the market looks at the item
    [SerializeField] private Texture2D icon;            // The icon of the item used in the inventory and market

    // Base Constructor
    public BaseItem(int id, string name, VulturaInstance.ItemType type, string description, VulturaInstance.ItemRarity rarity, Texture2D icon, float weight, int galacticPrice)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.description = description;
        this.rarity = rarity;
        this.icon = icon;
        this.weight = weight;
        this.galacticPrice = galacticPrice;
    }

    public int Id {
        get
        {
            return this.id;
        }
    }

    public string Name
    {
        get
        {
            return this.name;
        }
    }

    public VulturaInstance.ItemType Type
    {
        get
        {
            return this.type;
        }

        set
        {
            this.type = value;
        }
    }

    public string Description
    {
        get
        {
            return this.description;
        }
    }

    public VulturaInstance.ItemRarity Rarity
    {
        get
        {
            return this.rarity;
        }
    }

    public Texture2D Icon
    {
        get
        {
            return this.icon;
        }

    }

    public float Weight
    {
        get
        {
            return this.weight;
        }
    }

    public int GalacticPrice
    {
        get
        {
            return this.galacticPrice;
        }
    }
}
