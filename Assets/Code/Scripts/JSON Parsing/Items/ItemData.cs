using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string name;
    public RangeValue weight;
    public float galactic_price_modifier;
    public string icon_tag;
    public StatisticValue[] main_stats;
    public Override overrides;
}
