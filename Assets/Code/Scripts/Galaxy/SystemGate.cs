using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemGate
{
    public StarSystem destination;
    public Vector3 pos;

    public SystemGate(StarSystem destination, Vector3 pos)
    {
        this.destination = destination;
        this.pos = pos;
    }
}