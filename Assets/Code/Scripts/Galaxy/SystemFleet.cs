using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemFleet
{
    public Fleet fleet;
    public Vector3 originCoords;

    public SystemFleet(Fleet fleet, Vector3 originCoords)
    {
        this.fleet = fleet;
        this.originCoords = originCoords;
    }
}