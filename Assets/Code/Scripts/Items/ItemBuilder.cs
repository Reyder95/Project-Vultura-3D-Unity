using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryItem
{
    public Category category;
    public ItemData itemData;

    public CategoryItem(Category category, ItemData itemData)
    {
        this.category = category;
        this.itemData = itemData;
    }
}

public class ItemBuilder
{
    Dictionary<string, Func<CategoryItem, BaseItem>> itemBuilders;

    public BaseItem InvokeBuilder(string key, CategoryItem itemData)
    {
        Func<CategoryItem, BaseItem> func = itemBuilders[key];
        return func(itemData);
    }

    public void InitializeBuilder()
    {
        itemBuilders = new Dictionary<string, Func<CategoryItem, BaseItem>>();
        itemBuilders.Add("chaingun", categoryItem => BuildChaingun(categoryItem.category, categoryItem.itemData));
        itemBuilders.Add("rocket_launcher", categoryItem => BuildRocketLauncher(categoryItem.category, categoryItem.itemData));
        itemBuilders.Add("cargo_expander", categoryItem => BuildCargoExpander(categoryItem.category, categoryItem.itemData));
        itemBuilders.Add("trade_good", categoryItem => BuildTradeGood(categoryItem.category, categoryItem.itemData));
        itemBuilders.Add("ore", categoryItem => BuildOre(categoryItem.category, categoryItem.itemData));
    }

    private BaseItem BuildChaingun(Category categoryData, ItemData itemData)
    {
        VulturaInstance.ItemRarity itemRarity = Randomizer.GenerateItemRarity();
        float weight = UnityEngine.Random.Range(itemData.weight.min, itemData.weight.max);
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

    private BaseItem BuildRocketLauncher(Category categoryData, ItemData itemData)
    {
        VulturaInstance.ItemRarity itemRarity = Randomizer.GenerateItemRarity();
        float weight = UnityEngine.Random.Range(itemData.weight.min, itemData.weight.max);
        
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

    private BaseItem BuildCargoExpander(Category categoryData, ItemData itemData)
    {
        VulturaInstance.ItemRarity itemRarity = Randomizer.GenerateItemRarity();
        float weight = UnityEngine.Random.Range(itemData.weight.min, itemData.weight.max);
        
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

    private BaseItem BuildTradeGood(Category categoryData, ItemData itemData)
    {
        float weight = UnityEngine.Random.Range(itemData.weight.min, itemData.weight.max);

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

    private BaseItem BuildOre(Category categoryData, ItemData itemData)
    {
        float weight = itemData.weight.min;

        BaseItem generatedItem = new Ore(
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
}
