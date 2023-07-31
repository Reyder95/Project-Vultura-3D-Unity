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
    public static JSONGalaxyList GalaxyList;

    // Dictionaries
    public static Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();
    public static Dictionary<string, FacilityData> facilityDictionary = new Dictionary<string, FacilityData>();
    public static Dictionary<string, Category> categoryDictionary = new Dictionary<string, Category>();
    public static Dictionary<string, Type> typeDictionary = new Dictionary<string, Type>();
    public static Dictionary<string, StatsData> statDictionary = new Dictionary<string, StatsData>();
    public static Dictionary<string, MainStat> mainStatDictionary = new Dictionary<string, MainStat>();

    public static Dictionary<string, ItemData[]> basesFromCategory = new Dictionary<string, ItemData[]>();

    public static ItemData[] itemPool;
    public static StatsData[] statPool;

    public static void LoadData()
    {
        var categoryFile = Resources.Load<TextAsset>("JSON/Items/Categories");
        JSONCategories TempCategories = JsonUtility.FromJson<JSONCategories>(categoryFile.text);

        foreach (Category category in TempCategories.data)
        {
            categoryDictionary.Add(category.key, category);

            var jsonFile = Resources.Load<TextAsset>("JSON/Items/" + category.bases_file);
            JSONItems itemData = JsonUtility.FromJson<JSONItems>(jsonFile.text);

            if (itemPool != null)
                itemPool = CombineData(itemPool, itemData.data);
            else
                itemPool = itemData.data;

            foreach (ItemData loopData in itemData.data)
            {
                loopData.linking_key = itemData.linking_key;
                itemDictionary.Add(loopData.key, loopData);
            }

            basesFromCategory.Add(category.key, itemData.data);
        }

        var typeFile = Resources.Load<TextAsset>("JSON/Items/Types");
        JSONTypes TempTypes = JsonUtility.FromJson<JSONTypes>(typeFile.text);

        foreach (Type typeData in TempTypes.data)
        {
            typeDictionary.Add(typeData.key, typeData);
        }

        var facilityFile = Resources.Load<TextAsset>("JSON/Items/Facilities");
        JSONFacilities FacilityTypes = JsonUtility.FromJson<JSONFacilities>(facilityFile.text);

        foreach (FacilityData facility in FacilityTypes.data)
        {
            facilityDictionary.Add(facility.key, facility);
        }

        var statsFile = Resources.Load<TextAsset>("JSON/Items/Stats");
        JSONStats TempStats = JsonUtility.FromJson<JSONStats>(statsFile.text);

        statPool = TempStats.data;

        foreach (StatsData stats in TempStats.data)
        {
            statDictionary.Add(stats.key, stats);
        }

        var mainStatsFile = Resources.Load<TextAsset>("JSON/Items/MainStats");
        JSONMainStats TempMainStats = JsonUtility.FromJson<JSONMainStats>(mainStatsFile.text);

        foreach (MainStat mainStats in TempMainStats.data)
        {
            mainStatDictionary.Add(mainStats.key, mainStats);
        }

        var galaxyListFile = Resources.Load<TextAsset>("JSON/Galaxies/GalaxyList");
        JSONGalaxyList TempGalaxyList = JsonUtility.FromJson<JSONGalaxyList>(galaxyListFile.text);

        FillStatBucket();
    }

    private static void FillStatBucket()
    {
        foreach (var item in statDictionary)
        {
            StatBuckets.AddStat(item.Value.key, item.Value.tiers);
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
        if (facilityDictionary.TryGetValue(key, out FacilityData value))
            return value;

        return null;
        
    }

    public static Category FindCategoryByKey(string key)
    {
        if (categoryDictionary.TryGetValue(key, out Category value))
            return value;
        
        return null;
    }

    public static List<ItemData> FindBasesByCategory(Category category)
    {

        if (basesFromCategory.TryGetValue(category.key, out ItemData[] values))
            return new List<ItemData>(values);

        return null;
    }

    public static List<Category> FindCategoriesByType(Type type)
    {
        List<Category> categories = new List<Category>();

        foreach (var item in categoryDictionary)
        {
            if (item.Value.type == type.key)
                categories.Add(item.Value);
        }

        return categories;
    }

    public static Type FindTypeByKey(string key)
    {
        if (typeDictionary.TryGetValue(key, out Type value))
            return value;
        
        return null;
    }

    public static ItemData FindBaseByKey(string key)
    {
        if (itemDictionary.TryGetValue(key, out ItemData value))
            return value;

        return null;
    }

    public static MainStat FindMainStatByKey(string key)
    {
        if (mainStatDictionary.TryGetValue(key, out MainStat value))
            return value;

        return null;
    }

    public static StatsData FindStatByKey(string key)
    {
        if (statDictionary.TryGetValue(key, out StatsData value))
            return value;

        return null;
    }
}
