using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemEntity
{
    public string name;
    public string type;
    Vector3 pos;
    bool moveableEntity;
    public List<SystemEntity> subEntities = new List<SystemEntity>();
    public BaseSelectable entity;
    public SelectableEntity aboveEntity;
    public SystemEntity mainEntity = null;

    public SystemEntity(Vector3 pos, BaseSelectable entity, string name, bool moveableEntity, string type)
    {
        this.pos = pos;
        this.entity = entity;
        this.name = name;
        this.moveableEntity = moveableEntity;
        this.type = type;
    }

    public Vector3 GetPosition()
    {
        if (this.moveableEntity)
            return this.entity.selectableObject.transform.position;
        else
            return this.pos;
    }

    public void SetEntity(SelectableEntity aboveEntity)
    {
        this.aboveEntity = aboveEntity;
    }
}