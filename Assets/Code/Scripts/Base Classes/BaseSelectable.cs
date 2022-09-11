using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSelectable
{
    public GameObject selectableObject;

    private string faction;
    private string selectableName;
    private string type;
    private bool selected = false;
    private bool mainSelected = false;

    public BaseSelectable(string faction, string selectableName, string type)
    {
        this.faction = faction;
        this.selectableName = selectableName;
        this.type = type;
    }

    public void SetObject(GameObject selectableObject)
    {
        this.selectableObject = selectableObject;
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
