using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeGood : UnuseableItem
{
    //private GameObject prefab;
    public TradeGood(int id, string name, string description, Texture2D icon) : base(id, name, VulturaInstance.ItemType.Trade_Good, description, VulturaInstance.ItemRarity.Common, icon)
    {

    }

    // public void OnLoadDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    // {
    //     this.prefab = obj.Result;
    // }

    // public GameObject Prefab
    // {
    //     get
    //     {
    //         return this.prefab;
    //     }
    // }
}
