using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemManager
{
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
        int randInt = Random.Range(0, JSONDataHandler.Items.data.Length);
        ItemData itemData = JSONDataHandler.Items.data[randInt];
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
        if (itemData.linking_key == "chaingun")
        {
            VulturaInstance.ItemRarity itemRarity = Randomizer.GenerateItemRarity();
            int weight = Random.Range(itemData.weight.min, itemData.weight.max);

            StatHandler statHandler = new StatHandler();
            statHandler.BuildStatList(itemRarity, itemData.main_stats);

            BaseItem generatedItem = new Chaingun(
                itemData.key, 
                itemData.name, 
                categoryData.description, 
                itemRarity, 
                null, 
                categoryData.stats, 
                itemData.main_stats, 
                categoryData.bool_attributes, 
                categoryData.list_attributes, 
                itemData.overrides, 
                weight, 
                (int)Mathf.Floor((float)categoryData.galactic_price_base * itemData.galactic_price_modifier),
                statHandler
                );

            return generatedItem;
        }
        else if (itemData.linking_key == "rocket_launcher")
        {
            VulturaInstance.ItemRarity itemRarity = Randomizer.GenerateItemRarity();
            int weight = Random.Range(itemData.weight.min, itemData.weight.max);

            StatHandler statHandler = new StatHandler();
            statHandler.BuildStatList(itemRarity, itemData.main_stats);

            BaseItem generatedItem = new RocketLauncher(
                itemData.key, 
                itemData.name, 
                categoryData.description, 
                itemRarity, 
                null, 
                categoryData.stats, 
                itemData.main_stats, 
                categoryData.bool_attributes, 
                categoryData.list_attributes, 
                itemData.overrides, 
                weight, 
                (int)Mathf.Floor((float)categoryData.galactic_price_base * itemData.galactic_price_modifier),
                statHandler
                );

            return generatedItem;
        }
        else if (itemData.linking_key == "cargo_expander")
        {
            VulturaInstance.ItemRarity itemRarity = Randomizer.GenerateItemRarity();
            int weight = Random.Range(itemData.weight.min, itemData.weight.max);

            StatHandler statHandler = new StatHandler();
            statHandler.BuildStatList(itemRarity, itemData.main_stats);

            BaseItem generatedItem = new CargoExpander(
                itemData.key, 
                itemData.name, 
                categoryData.description, 
                itemRarity, 
                null, 
                categoryData.stats, 
                itemData.main_stats, 
                categoryData.bool_attributes, 
                categoryData.list_attributes, 
                itemData.overrides, 
                weight, 
                (int)Mathf.Floor((float)categoryData.galactic_price_base * itemData.galactic_price_modifier),
                statHandler
                );

            return generatedItem;
        }
        else if (categoryData.type == "trade_good")
        {
            int weight = Random.Range(itemData.weight.min, itemData.weight.max);

            BaseItem generatedItem = new TradeGood(
                itemData.key, 
                itemData.name, 
                categoryData.description, 
                null,  
                weight, 
                (int)Mathf.Floor((float)categoryData.galactic_price_base * itemData.galactic_price_modifier),
                categoryData.stackable
                );

            return generatedItem;
        }

        return null;
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
