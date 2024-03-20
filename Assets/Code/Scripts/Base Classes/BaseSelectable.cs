using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Every selectable item is of type base selectable.
public class BaseSelectable
{
    public GameObject selectableObject; // The game object that is selectable via clicking.

    private string faction;     // The faction of this base selectable. This may be a faction ship, or a station. Or an asteroid with no faction representation
    private string selectableName;      // The name of the selectable. This is what is shown on the selection table.
    private string type;            // The type of selectable. Ship, Mining Station, etc
    private bool selected = false;      // Is this a selected item? If so, outline it.
    private bool mainSelected = false;  // If it's a main selected item, outline it a different color.
    public SystemEntity entity;

    public BaseSelectable(string faction, string selectableName, string type)
    {
        this.faction = faction;
        this.selectableName = selectableName;
        this.type = type;
    }

    // Sets the object this selectable is related to.
    public void SetObject(GameObject selectableObject)
    {
        this.selectableObject = selectableObject;
    }

    public void SetEntity(SystemEntity entity)
    {
        this.entity = entity;
    }

    public string Faction 
    {
        get
        {
            return this.faction;
        }
    }

    public string SelectableName
    {
        get
        {
            return this.selectableName;
        }

        set
        {
            this.selectableName = value;
        }
    }

    public string Type
    {
        get
        {
            return this.type;
        }
    }

    public bool MainSelected
    {
        get
        {
            return this.mainSelected;
        }
        set
        {
            this.mainSelected = value;
        }
    }

    public bool Selected
    {
        get
        {
            return this.selected; 
        }

        set
        {
            this.selected = value;
        }
    }
}
