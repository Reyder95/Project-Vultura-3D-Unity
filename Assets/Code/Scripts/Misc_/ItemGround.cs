using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class ItemGround : MonoBehaviour
{
    public VisualTreeAsset itemTooltipStat;

    private InventoryItem item = null;

    public GameObject canvas;
    public GameObject itemName;

    public VisualElement rootVisualElement;
    

    public void Update()
    {
        Transform camPos = Camera.main.transform;

        canvas.transform.LookAt(canvas.transform.position + camPos.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        if (rootVisualElement != null)
        {
            //rootVisualElement.Q<VisualElement>("item-tooltip").
        }
    }


    public void SetItem(InventoryItem item)
    {
        TMP_Text itemText = canvas.transform.GetChild(0).GetComponent<TMP_Text>();

        this.item = item;

        this.gameObject.GetComponent<Renderer>().material.SetColor("_BaseColor", VulturaInstance.GenerateItemColor(item.item.Rarity));
        this.gameObject.GetComponent<Outline>().OutlineColor = VulturaInstance.GenerateItemColor(item.item.Rarity);
        itemText.color = VulturaInstance.GenerateItemColor(item.item.Rarity);

        string itemNameText = string.Empty;

        if (item.quantity > 1)
            itemNameText = itemNameText + item.quantity.ToString() + "x ";

        itemNameText = itemNameText + item.item.Name;

        itemText.text = itemNameText;

        rootVisualElement = this.gameObject.GetComponent<UIDocument>().rootVisualElement;
        rootVisualElement.RegisterCallback<PointerMoveEvent>(ev => {
            VisualElement itemTooltip = rootVisualElement.Q<VisualElement>("item-tooltip");
            itemTooltip.style.top = ev.position.y;
            itemTooltip.style.left = ev.position.x;
        });

        VisualElement itemTooltip = rootVisualElement.Q<VisualElement>("item-tooltip");

        Color32 rarityColor = VulturaInstance.GenerateItemColor(item.item.Rarity);
        itemTooltip.style.borderBottomColor = new StyleColor(rarityColor);
        itemTooltip.style.borderLeftColor = new StyleColor(rarityColor);
        itemTooltip.style.borderRightColor = new StyleColor(rarityColor);
        itemTooltip.style.borderTopColor = new StyleColor(rarityColor);
        itemTooltip.Q<Label>("item-name").text =  item.item.Name;
        itemTooltip.Q<Label>("item-name").style.color = new StyleColor(rarityColor);
        itemTooltip.Q<Label>("item-category").text = item.item.Category;
        itemTooltip.Q<Label>("item-rarity").text = VulturaInstance.enumStringParser(item.item.Rarity.ToString());
        itemTooltip.Q<Label>("item-rarity").style.color = new StyleColor(rarityColor);

        VisualElement affixesElement = itemTooltip.Q<VisualElement>("affixes");

        VisualElement mainStatElement = itemTooltip.Q<VisualElement>("main-stats");

        if (item.item.Rarity == VulturaInstance.ItemRarity.Common)
            affixesElement.style.display = DisplayStyle.None;
        else
            affixesElement.style.display = DisplayStyle.Flex;


        if (item.item is Module)
        {
            StatHandler itemStatHandler = (item.item as Module).StatHandler;

            affixesElement.Clear();
            mainStatElement.Clear();

            foreach (ItemStat mainStat in itemStatHandler.Main)
            {
                VisualElement statLabel = itemTooltipStat.Instantiate();
                statLabel.Q<Label>("item-stat").text = mainStat.ReturnStatDescription();
                mainStatElement.Add(statLabel);
            }

            foreach (ItemStat prefixStat in itemStatHandler.Prefixes)
            {
                VisualElement statLabel = itemTooltipStat.Instantiate();
                statLabel.Q<Label>("item-stat").text = prefixStat.ReturnStatDescription();
                affixesElement.Add(statLabel);
            }

            foreach (ItemStat suffixStat in itemStatHandler.Suffixes)
            {
                VisualElement statLabel = itemTooltipStat.Instantiate();
                statLabel.Q<Label>("item-stat").text = suffixStat.ReturnStatDescription();
                affixesElement.Add(statLabel);
            }
        }

        itemTooltip.style.position = Position.Absolute;
        itemTooltip.style.display = DisplayStyle.None;
    }

    public InventoryItem GetItem()
    {
        return item;
    }

    void OnMouseOver()
    {
        Vector2 pos = Input.mousePosition;

        VisualElement itemTooltip = rootVisualElement.Q<VisualElement>("item-tooltip");

        itemTooltip.pickingMode = PickingMode.Ignore;
        itemTooltip.pickingMode = PickingMode.Ignore;              
        itemTooltip.style.position = Position.Absolute;
        itemTooltip.style.display = DisplayStyle.Flex;

        if (!this.gameObject.GetComponent<Outline>().enabled)
            this.gameObject.GetComponent<Outline>().enabled = true;
    }

    void OnMouseExit()
    {
        rootVisualElement.Q<VisualElement>("item-tooltip").style.display = DisplayStyle.None;
        this.gameObject.GetComponent<Outline>().enabled = false;
    }
}
