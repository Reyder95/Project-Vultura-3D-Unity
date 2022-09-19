using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactoryComponent : MonoBehaviour
{
    private Dictionary<int, BaseItemFactory> itemFactoryDict = new Dictionary<int, BaseItemFactory>();

    public static ItemFactoryComponent Instance { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        itemFactoryDict.Add(1, new LuxuryGoodsFactory());
        itemFactoryDict.Add(2, new FreshFoodFactory());
        itemFactoryDict.Add(3, new FreshWaterFactory());
        itemFactoryDict.Add(4, new ChaingunFactory());
        itemFactoryDict.Add(5, new RocketLauncherFactory());
        itemFactoryDict.Add(6, new CargoExpanderFactory());
    }

    public Dictionary<int, BaseItemFactory> ItemFactoryDict
    {
        get
        {
            return this.itemFactoryDict;
        }
    }
}
