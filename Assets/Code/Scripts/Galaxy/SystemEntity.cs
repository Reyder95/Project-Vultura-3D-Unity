using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemEntity
{
    public string name;
    public string type;
    public VulturaInstance.EntityType actualType;
    Vector3 pos;
    public bool moveableEntity;
    public List<SystemEntity> subEntities = new List<SystemEntity>();
    public BaseSelectable entity;
    public SelectableEntity aboveEntity;
    public SystemEntity mainEntity = null;
    public List<SystemFleet> fleets = new List<SystemFleet>();

    public SystemEntity(Vector3 pos, BaseSelectable entity, string name, bool moveableEntity, string type)
    {
        this.pos = pos;
        this.entity = entity;
        this.name = name;
        this.moveableEntity = moveableEntity;
        this.type = type;

        if (type.Contains("Station"))
            this.actualType = VulturaInstance.EntityType.MINING_STATION;
        else if (type.Contains("Asteroid"))
            this.actualType = VulturaInstance.EntityType.ASTEROID_FIELD;
    }

    public Vector3 GetPosition()
    {
        if (this.entity != null)
        {
            if (this.entity.selectableObject != null)
            {
                if (this.moveableEntity)
                    return this.entity.selectableObject.transform.position;
                else
                    return this.pos;
            }
        }


        return this.pos;

    }

    public void SetEntity(SelectableEntity aboveEntity)
    {
        this.aboveEntity = aboveEntity;
    }
}