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

        // TODO -- Use folder from categories to make this easier

        ParseItemJSON("JSON/Items/Active Modules/Chainguns");
        ParseItemJSON("JSON/Items/Active Modules/RocketLaunchers");
        ParseItemJSON("JSON/Items/Passive Modules/CargoExpanders");
        ParseItemJSON("JSON/Items/Trade Goods/FreshFood");
        ParseItemJSON("JSON/Items/Trade Goods/FreshWater");
        ParseItemJSON("JSON/Items/Trade Goods/LuxuryGoods");

        Items.linking_key = null;
        
    }

    public static void ParseItemJSON(string filepath)
    {
        var jsonTextFile = Resources.Load<TextAsset>(filepath);
        JSONItems tempData = JsonUtility.FromJson<JSONItems>(jsonTextFile.text);

        foreach (ItemData itemData in tempData.data)
            itemData.linking_key = tempData.linking_key;

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

    public static Category FindCategoryByKey(string key)
    {
        foreach (Category category in Categories.data)
        {
            if (category.key == key)
                return category;
        }

        return null;
    }

    public static Type FindTypeByKey(string key)
    {
        foreach (Type type in Types.data)
        {
            if (type.key == key)
                return type;
        }
        
        return null;
    }

    public static ItemData FindBaseByKey(string key)
    {
        foreach (ItemData itemData in Items.data)
        {
            if (itemData.key == key)
                return itemData;
        }

        return null;
    }
}
