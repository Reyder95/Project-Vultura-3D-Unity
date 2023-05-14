using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountComponent : MonoBehaviour
{
    public GameObject turret;

    public GameObject miningTurretPrefab;

    public TurretComponent component;

    void Update()
    {
        if (component != null)
        {
            if (component.target != null)
            {
                this.gameObject.transform.LookAt(component.target.selectableObject.transform);
            }
        }
    }

    public void EquipTurret(ActiveModule turretModule)
    {
        this.turret = this.gameObject.transform.GetChild(0).gameObject; 

        this.component = this.turret.GetComponent<LaserTurretComponent>();

        this.component.turret = turretModule;

    }

    public void UseTurret(BaseSelectable target)
    {
        if (turret != null)
        {
            if (component.target == null)
            {
                component.target = target;

                if (component is LaserTurretComponent)
                {
                    (component as LaserTurretComponent).StartLaser();
                }
            }
            else
            {
                StopTurret();
            }

        }


    }

    public void StopTurret()
    {
        component.target = null;

        if (component is LaserTurretComponent)
        {
            Debug.Log("TEST!");
            (component as LaserTurretComponent).StopLaser();
        }
    }
}
