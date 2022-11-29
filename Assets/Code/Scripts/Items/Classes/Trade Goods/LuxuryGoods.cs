using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LuxuryGoods : TradeGood
{
    public LuxuryGoods() : base(1, "Luxury Goods", "Luxury goods that the people of the galaxy enjoy partaking in.", new Texture2D(128, 128), 5.6f, 18)
    {
        //Addressables.LoadAssetAsync<GameObject>("TestPrefab").Completed += base.OnLoadDone;
    }
}
