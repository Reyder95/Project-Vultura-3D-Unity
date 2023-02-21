using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FacilityData
{
    public string key;
    public string name;
    public FacilityValue[] produce;
    public FacilityValue[] consume;
}
