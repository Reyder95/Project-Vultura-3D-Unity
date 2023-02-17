using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Category
{
    public string key;
    public string description;
    public bool stackable;
    public int galactic_price_base;
    public string type;
    public string bases_file;
    public BoolValue[] bool_attributes;
    public ListValue[] list_attributes;
    public StatisticValue[] stats;
}
