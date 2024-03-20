using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SystemData
{
    public string system_name;
    public string star_type;
    public int size;
    public SystemPlanet[] planets;
    public SystemAsteroid[] asteroid_fields;
    public SystemGates[] gates;
}