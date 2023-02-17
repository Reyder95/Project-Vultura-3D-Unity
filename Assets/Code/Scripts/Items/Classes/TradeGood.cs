using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Trade goods. Their only purpose is for providing to stations or trading between stations.
public class TradeGood : UnuseableItem
{
    //private GameObject prefab;
    public TradeGood(
        int id, 
        string name, 
        string description, 
        Texture2D icon, 
        float weight, 
        int galacticPrice, 
        bool stackable
        ) : base(
            id, 
            name, 
            VulturaInstance.
            ItemType.Trade_Good, 
            description, 
            VulturaInstance.ItemRarity.Common, 
            icon, 
            weight, 
            galacticPrice, 
            stackable
            )
    {

    }
}
