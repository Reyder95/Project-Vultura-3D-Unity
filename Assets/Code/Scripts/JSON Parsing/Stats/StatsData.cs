using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatsData
{
    public string key;
    public string effector_key;
    public string modifier;
    public string helper_text;
    public string display_text;
    public string type;
    public string numType;
    public StatTier[] tiers;
    public int[] tierProbability;
}
