using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseItem
{
    [Header("Item Information")]
    [SerializeField] int id;
    [SerializeField] string name;
    [SerializeField] private VulturaInstance.ItemType type;
    [SerializeField] private string description;
    [SerializeField] private VulturaInstance.ItemRarity rarity;
    [SerializeField] private Texture2D icon;

    // Base Constructor
    public BaseItem(int id, string name, VulturaInstance.ItemType type, string description, VulturaInstance.ItemRarity rarity, Texture2D icon)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.description = description;
        this.rarity = rarity;
        this.icon = icon;
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
}
