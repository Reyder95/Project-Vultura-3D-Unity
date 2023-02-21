using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemManager
{
    private static ItemBuilder itemBuilder = new ItemBuilder();

    public static void InitializeItemBuilder()
    {
        itemBuilder.InitializeBuilder();
        //itemBuilder.InvokeBuilder("chaingun", new CategoryItem(JSONDataHandler.categoryDictionary["chaingun"], JSONDataHandler.itemDictionary["rusty_chaingun"]));
    }

    public static BaseItem GenerateSpecificBase(string baseKey)
    {
        try 
        {
            ItemData itemData = JSONDataHandler.FindBaseByKey(baseKey);
            Category categoryData = JSONDataHandler.FindCategoryByKey(itemData.linking_key);

            return GenerateItem(categoryData, itemData);
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log("Item was not found. Check if the correct base key was provided.");
            Debug.Log(ex);

            return null;
        }
    }

    public static BaseItem GenerateRandomItem()
    {
        int randInt = Random.Range(0, JSONDataHandler.itemPool.Length);
        ItemData itemData = JSONDataHandler.itemPool[randInt];
        Category categoryData = JSONDataHandler.FindCategoryByKey(itemData.linking_key);


        return GenerateItem(categoryData, itemData);
    }

    public static BaseItem GenerateRandomBaseFromCategory(string categoryKey)
    {
        try
        {
            Category categoryData = JSONDataHandler.FindCategoryByKey(categoryKey);
            List<ItemData> categoryBases = JSONDataHandler.FindBasesByCategory(categoryData);

            int randIndex = Random.Range(0, categoryBases.Count);

            return GenerateItem(categoryData, categoryBases[randIndex]);
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log("Item was not found. Check if the correct category key was provided.");
            Debug.Log(ex);

            return null;
        }
    }

    public static BaseItem GenerateRandomBaseFromType(string typeKey)
    {
        try 
        {
            Type typeData = JSONDataHandler.FindTypeByKey(typeKey);
            List<Category> typeCategories = JSONDataHandler.FindCategoriesByType(typeData);

            int randIntCat = Random.Range(0, typeCategories.Count);

            List<ItemData> itemData = JSONDataHandler.FindBasesByCategory(typeCategories[randIntCat]);

            int randIntItem = Random.Range(0, itemData.Count);

            return GenerateItem(typeCategories[randIntCat], itemData[randIntItem]);

        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log("Item was not found. Check if the correct category key was provided.");
            Debug.Log(ex);

            return null;
        }
    }

    public static BaseItem GenerateItem(Category categoryData, ItemData itemData)
    {
        return itemBuilder.InvokeBuilder(categoryData.key, new CategoryItem(categoryData, itemData));
    }

    public static Facility GenerateFacility(string facilityKey)
    {
        FacilityData facilityData = JSONDataHandler.FindFacilityByKey(facilityKey);

        List<FacilityItem> produce = new List<FacilityItem>();
        List<FacilityItem> consume = new List<FacilityItem>();

        foreach (FacilityValue facilityValue in facilityData.produce)
        {
            ItemData itemData = JSONDataHandler.FindBaseByKey(facilityValue.key);
            Category categoryData = JSONDataHandler.FindCategoryByKey(itemData.linking_key);
            produce.Add(new FacilityItem(GenerateItem(categoryData, itemData), facilityValue.value));
        }

        foreach (FacilityValue facilityValue in facilityData.consume)
        {
            ItemData itemData = JSONDataHandler.FindBaseByKey(facilityValue.key);
            Category categoryData = JSONDataHandler.FindCategoryByKey(itemData.linking_key);
            consume.Add(new FacilityItem(GenerateItem(categoryData, itemData), facilityValue.value));
        }

        return new Facility(facilityKey, produce, consume, facilityData.name);
    }
}
