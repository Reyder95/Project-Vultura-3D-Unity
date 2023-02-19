using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This seems a bit inefficient with the constant looping needing to be done across items. To make this more efficient, I might scrap the 3 types of variables
// and instead grab data directly from the resources in each function since I can cut things down by their folder and category.
public static class JSONDataHandler
{
    public static JSONTypes Types;
    public static JSONCategories Categories;
    public static JSONItems Items;
    public static JSONFacilities Facilities;
    public static JSONStats Stats;
    public static JSONMainStats MainStats;

    public static void LoadData()
    {
        var jsonTextFile = Resources.Load<TextAsset>("JSON/Items/Types");
        Types = JsonUtility.FromJson<JSONTypes>(jsonTextFile.text);

        jsonTextFile = Resources.Load<TextAsset>("JSON/Items/Categories");
        Categories = JsonUtility.FromJson<JSONCategories>(jsonTextFile.text);

        jsonTextFile = Resources.Load<TextAsset>("JSON/Items/Facilities");
        Facilities = JsonUtility.FromJson<JSONFacilities>(jsonTextFile.text);

        jsonTextFile = Resources.Load<TextAsset>("JSON/Items/Stats");
        Stats = JsonUtility.FromJson<JSONStats>(jsonTextFile.text);

        FillStatBucket();

        jsonTextFile = Resources.Load<TextAsset>("JSON/Items/MainStats");
        MainStats = JsonUtility.FromJson<JSONMainStats>(jsonTextFile.text);

        // TODO -- Use folder from categories to make this easier

        ParseItemJSON("JSON/Items/Active Modules/Chainguns");
        ParseItemJSON("JSON/Items/Active Modules/RocketLaunchers");
        ParseItemJSON("JSON/Items/Passive Modules/CargoExpanders");
        ParseItemJSON("JSON/Items/Trade Goods/TradeGoods");

        Items.linking_key = null;
    }

    private static void FillStatBucket()
    {
        foreach (StatsData statsData in Stats.data)
        {
            StatBuckets.AddStat(statsData.key, statsData.tiers);
        }
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

    public static FacilityData FindFacilityByKey(string key)
    {
        foreach (FacilityData facilityData in Facilities.data)
        {
            if (facilityData.key == key)
                return facilityData;
        }

        return null;
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

    public static List<ItemData> FindBasesByCategory(Category category)
    {
        List<ItemData> bases = new List<ItemData>();

        foreach (ItemData itemData in Items.data)
        {
            if (itemData.linking_key == category.key)
                bases.Add(itemData);
        }

        return bases;
    }

    public static List<Category> FindCategoriesByType(Type type)
    {
        List<Category> categories = new List<Category>();

        foreach (Category category in Categories.data)
        {
            if (category.type == type.key)
                categories.Add(category);
        }

        return categories;
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

    public static MainStat FindMainStatByKey(string key)
    {
        foreach (MainStat mainStat in MainStats.data)
        {
            if (mainStat.key == key)
                return mainStat;
        }

        return null;
    }

    public static StatsData FindStatByKey(string key)
    {
        foreach (StatsData statsData in Stats.data)
        {
            if (statsData.key == key)
                return statsData;
        }

        return null;
    }
}
