using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptableScript : MonoBehaviour
{
    public BaseItem item;

    void Start()
    {
        item = new LuxuryGoods();
    }

    void Update()
    {
        Debug.Log((item as TradeGood).Prefab);
    }
}
