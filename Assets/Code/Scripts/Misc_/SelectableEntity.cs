using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableEntity
{
    public string name;
    public string faction;
    public SystemEntity entity;
    public string type;
    public bool mainSelected;
    public bool selected;

    public SelectableEntity(string name, string faction, SystemEntity entity, string type)
    {
        this.name = name;
        this.faction = faction;
        this.entity = entity;
        this.type = type;
    }
}