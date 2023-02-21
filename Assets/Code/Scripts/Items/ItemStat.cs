using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStat
{
    private string key;
    private string effectorKey;
    private string modifier;
    private string helperText;
    private string displayText;
    private float value;
    private int tier;

    public ItemStat(
        string key, 
        string effectorKey,
        string modifier,
        string helperText,
        string displayText,
        float value,
        int tier
        )
    {
        this.key = key;
        this.effectorKey = effectorKey;
        this.modifier = modifier;
        this.helperText = helperText;
        this.displayText = displayText;
        this.value = value;
        this.tier = tier;
    }

    public string ReturnStatDescription()
    {
        return displayText.Replace("{{value}}", "<color=#9CA3F5><b>" + value.ToString() + "</b></color>");
    }

    public string Key
    {
        get
        {
            return this.key;
        }
    }

    public string EffectorKey
    {
        get
        {
            return this.effectorKey;
        }
    }

    public string Modifier
    {
        get
        {
            return this.modifier;
        }
    }

    public string HelperText
    {
        get
        {
            return this.helperText;
        }
    }

    public string DisplayText
    {
        get
        {
            return this.displayText;
        }
    }

    public float Value
    {
        get
        {
            return this.value;
        }
    }

    public int Tier
    {
        get
        {
            return this.tier;
        }
    }
}
