using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wrapper for a station to provide information upon that station. Such as coordinates in its own world, perhaps the system and the sector it is in, too, if needed.
public class StationWrapper
{
    Vector3 position;
    BaseStation station;

    public StationWrapper(Vector3 position, BaseStation station)
    {
        this.position = position;
        this.station = station;
    }
}

