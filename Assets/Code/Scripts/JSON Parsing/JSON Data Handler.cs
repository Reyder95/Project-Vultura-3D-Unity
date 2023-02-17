using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JSONDataHandler
{
    public static JSONTypes Types;
    public static JSONCategories Categories;
    public static JSONItems Items;

    public static void LoadData()
    {
        var jsonTextFile = Resources.Load<TextAsset>("JSON/Items/Types");
        Types = JsonUtility.FromJson<JSONTypes>(jsonTextFile.text);

        jsonTextFile = Resources.Load<TextAsset>("JSON/Items/Categories");
        Categories = JsonUtility.FromJson<JSONCategories>(jsonTextFile.text);

        ParseItemJSON("JSON/Items/Active Modules/Chainguns");
        ParseItemJSON("JSON/Items/Active Modules/RocketLaunchers");
        ParseItemJSON("JSON/Items/Passive Modules/CargoExpanders");
        ParseItemJSON("JSON/Items/Trade Goods/FreshFood");
        ParseItemJSON("JSON/Items/Trade Goods/FreshWater");
        ParseItemJSON("JSON/Items/Trade Goods/LuxuryGoods");
        
        Debug.Log(Items.data.Length);
        
    }

    public static void ParseItemJSON(string filepath)
    {
        var jsonTextFile = Resources.Load<TextAsset>(filepath);
        JSONItems tempData = JsonUtility.FromJson<JSONItems>(jsonTextFile.text);
        if (Items != null)
            Items.data = CombineData(Items.data, tempData.data);
        else
            Items = tempData;
    }

    public static ItemData[] CombineData(ItemData[] arr1, ItemData[] arr2)
    {
        ItemData[] tempArray = new ItemData[arr1.Length + arr2.Length];
        arr1.CopyTo(tempArray, 0);
        arr2.CopyTo(tempArray, arr1.Length);
        return tempArray;
    }
}
