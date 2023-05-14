using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component that attaches to a turret prefab, to handle turret stuff
public class TurretComponent : MonoBehaviour
{
    public ActiveModule turret = null;
    public BaseSelectable target = null;

}
