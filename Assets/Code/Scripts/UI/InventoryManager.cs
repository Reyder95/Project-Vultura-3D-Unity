using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public VisualTreeAsset inventoryRow;
    public VisualTreeAsset inventoryItem;
    public VisualTreeAsset itemTooltip;
    public VisualTreeAsset itemTooltipStat;

    bool isDragging = false;

    List<VisualElement> items = new List<VisualElement>();

    VisualElement rootVisualElement;
    ScrollView inventoryScroller;
    VisualElement inventory;

    VisualElement currentDraggedElement;
    VisualElement currentElementOver;

    VisualElement visualDragger;
    VisualElement tempTooltip;

    private UnityAction inventoryListener;

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

        inventoryListener = new UnityAction(InventoryEvent);
    }

    void OnDisable()
    {
        EventManager.StopListening("Inventory Modified", inventoryListener);
    }

    void OnEnable()
    {
        EventManager.StartListening("Inventory Modified", inventoryListener);

        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;   
        inventoryScroller = rootVisualElement.Q<ScrollView>("inventory-scroller");
        inventory = rootVisualElement.Q<VisualElement>("base-content");

        rootVisualElement.RegisterCallback<PointerUpEvent>(ev => {

            if (isDragging)
            {
                isDragging = false;

                visualDragger.Q<VisualElement>("inventory-item").style.visibility = Visibility.Hidden;

                VisualElement overlappingElement = FindOverlappingItem();

                if (overlappingElement != null)
                {
                    VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Swap((int)currentDraggedElement.userData, (int)overlappingElement.userData);
                    OpenInventory();
                }
                else
                {
                    if (currentDraggedElement != null)
                        currentDraggedElement.Q<VisualElement>("inventory-item").visible = true;
                }
            }
        });

        rootVisualElement.style.display = DisplayStyle.None;

    }

    public void HandleInventory()
    {
        UIDocument inventoryDocument = GetComponent<UIDocument>();

        if (rootVisualElement.style.display == DisplayStyle.Flex)
            CloseInventory();
        else
            OpenInventory();
    }

    public VisualElement FindOverlappingItem()
    {
        foreach (VisualElement item in items)
        {
            if (visualDragger.worldBound.Overlaps(item.worldBound))
                return item;
        }

        return null;
    }

    public void InventoryEvent()
    {
        try
        {
            if (tempTooltip != null)
            {
                rootVisualElement.Remove(tempTooltip);
                tempTooltip = null; 
            }
            DisplayInventory(); 
 
        } catch (System.NullReferenceException ex)
        {
            Debug.Log("Inventory isn't loaded yet!");
        }

    }

    public void OpenInventory()
    {
        DisplayInventory();
        rootVisualElement.style.display = DisplayStyle.Flex;
    }

    private void DisplayInventory()
    {
        try
        {
            items.Clear();
            inventoryScroller.Clear();
            Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
            visualDragger = rootVisualElement.Q<VisualElement>("ghost-item");

            VisualElement currentRow = inventoryRow.Instantiate();

            for (int i = 0; i < playerInventory.itemList.Count; i++)
            {
                if (i % 6 == 0)
                {
                    currentRow = inventoryRow.Instantiate();
                    inventoryScroller.Add(currentRow);
                }

                VisualElement item = inventoryItem.Instantiate();
                item.Q<VisualElement>("inventory-item").visible = true;
                item.style.width = Length.Percent(16.6f);
                item.userData = i;
                item.Q<Label>("item-quantity").text = playerInventory.itemList[i].quantity.ToString();
                item.Q<Label>("item-name").text = playerInventory.itemList[i].item.Name;
                item.RegisterCallback<PointerDownEvent>(ev => {
                    VisualElement eventTarget = (ev.currentTarget as VisualElement);
                    currentDraggedElement = eventTarget;
                    currentDraggedElement.Q<VisualElement>("inventory-item").visible = false;

                    visualDragger.Q<Label>("item-quantity").text = eventTarget.Q<Label>("item-quantity").text;
                    visualDragger.Q<Label>("item-name").text = eventTarget.Q<Label>("item-name").text;

                    visualDragger.pickingMode = PickingMode.Ignore;
                    visualDragger.Q<VisualElement>("inventory-item").pickingMode = PickingMode.Ignore;              
                    visualDragger.style.position = Position.Absolute;
                    visualDragger.Q<VisualElement>("inventory-item").style.visibility = Visibility.Visible;
                    visualDragger.style.top = ev.position.y - visualDragger.layout.height / 2;
                    visualDragger.style.left = ev.position.x - visualDragger.layout.width / 2;
                    isDragging = true;

                    visualDragger.RegisterCallback<PointerMoveEvent>(ev => {
                        if (!isDragging)
                            return;
                            
                        (ev.currentTarget as VisualElement).style.top = ev.position.y - (ev.currentTarget as VisualElement).layout.height / 2;
                        (ev.currentTarget as VisualElement).style.left = ev.position.x - (ev.currentTarget as VisualElement).layout.width / 2;
                    });

                    visualDragger.RegisterCallback<PointerLeaveEvent>(ev => {
                        if (!isDragging)
                            return;

                        (ev.currentTarget as VisualElement).style.top = ev.position.y - (ev.currentTarget as VisualElement).layout.height / 2;
                        (ev.currentTarget as VisualElement).style.left = ev.position.x - (ev.currentTarget as VisualElement).layout.width / 2;
                    });

                    visualDragger.RegisterCallback<PointerUpEvent>(ev => {
                        (ev.currentTarget as VisualElement).Q<VisualElement>("inventory-item").style.visibility = Visibility.Hidden;
                    });
                });
                item.RegisterCallback<PointerEnterEvent>(ev => {
                    bool newTooltip = false;
                    if (tempTooltip == null)
                    {
                        newTooltip = true;
                        tempTooltip = itemTooltip.Instantiate();
                    }

                    Color32 rarityColor = VulturaInstance.GenerateItemColor(playerInventory.itemList[(int)(ev.currentTarget as VisualElement).userData].item.Rarity);
                    tempTooltip.Q<VisualElement>("item-tooltip").style.borderBottomColor = new StyleColor(rarityColor);
                    tempTooltip.Q<VisualElement>("item-tooltip").style.borderLeftColor = new StyleColor(rarityColor);
                    tempTooltip.Q<VisualElement>("item-tooltip").style.borderRightColor = new StyleColor(rarityColor);
                    tempTooltip.Q<VisualElement>("item-tooltip").style.borderTopColor = new StyleColor(rarityColor);
                    tempTooltip.Q<Label>("item-name").text = playerInventory.itemList[(int)(ev.currentTarget as VisualElement).userData].item.Name;
                    tempTooltip.Q<Label>("item-name").style.color = new StyleColor(rarityColor);
                    tempTooltip.Q<Label>("item-category").text = playerInventory.itemList[(int)(ev.currentTarget as VisualElement).userData].item.Category;
                    tempTooltip.Q<Label>("item-rarity").text = VulturaInstance.enumStringParser(playerInventory.itemList[(int)(ev.currentTarget as VisualElement).userData].item.Rarity.ToString());
                    tempTooltip.Q<Label>("item-rarity").style.color = new StyleColor(rarityColor);

                    BaseItem currItem = playerInventory.itemList[(int)(ev.currentTarget as VisualElement).userData].item;

                    VisualElement affixesElement = tempTooltip.Q<VisualElement>("affixes");

                    VisualElement mainStatElement = tempTooltip.Q<VisualElement>("main-stats");

                    if (currItem.Rarity == VulturaInstance.ItemRarity.Common)
                        affixesElement.style.display = DisplayStyle.None;
                    else
                        affixesElement.style.display = DisplayStyle.Flex;


                    if (currItem is Module)
                    {
                        StatHandler itemStatHandler = (currItem as Module).StatHandler;

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

                    tempTooltip.pickingMode = PickingMode.Ignore;
                    tempTooltip.style.position = Position.Absolute;
                    tempTooltip.style.top = ev.position.y;
                    tempTooltip.style.left = ev.position.x;
                    Vector3 pos = Input.mousePosition;
                    rootVisualElement.Add(tempTooltip);
                });
                item.RegisterCallback<PointerMoveEvent>(ev => {
                    if (tempTooltip != null)
                    {
                        tempTooltip.style.top = ev.position.y;
                        tempTooltip.style.left = ev.position.x;
                    }

                });
                item.RegisterCallback<PointerLeaveEvent>(ev => {
                    rootVisualElement.Remove(tempTooltip);
                    tempTooltip = null;
                });

                items.Add(item);

                currentRow.Add(item);
            }

            float percentUsed = (playerInventory.currCargo / VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().shipStats.baseCargo) * 100;

            rootVisualElement.Q<Label>("cargo-capacity").text = playerInventory.currCargo.ToString() + " / " + VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().shipStats.baseCargo.ToString() + " (" + percentUsed.ToString("N1") + "%)";

            rootVisualElement.Q<VisualElement>("bar-percent").style.width = Length.Percent(percentUsed);
        } catch (System.NullReferenceException ex)
        {
            Debug.Log("Inventory isn't loaded yet!");
        }

    }

    public void CloseInventory()
    {
        rootVisualElement.style.display = DisplayStyle.None;
    }
}
