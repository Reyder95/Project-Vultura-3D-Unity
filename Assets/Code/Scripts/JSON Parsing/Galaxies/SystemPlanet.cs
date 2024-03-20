using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SystemPlanet
{
    public string name;
    public string type;
    public SystemPlanet[] moons;
    public SystemBody[] bodies;
    public int radius;
    public int angle;
    public int height;
    public string size;
}