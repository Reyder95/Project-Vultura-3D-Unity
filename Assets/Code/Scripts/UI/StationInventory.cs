using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class StationInventory : OSUIHandler
{

    private VisualElement inventoryVisualElement;

    private VisualElement inventoryScroller;
    private ListView shipScroller;

    public VisualElement tempTooltip;

    public UnityAction refreshListener;

    public bool storageItems = true;

    public VisualElement itemButton;
    public VisualElement shipButton;

    public override void SetTaggedReferences(VisualElement screen, StationUI station)
    {
        refreshListener = new UnityAction(DisplayInventoryEvent);
        uiComponent = station;

        inventoryVisualElement = screen.Q<VisualElement>("station-inventory");
        inventoryScroller = inventoryVisualElement.Q<VisualElement>("inventory-scroller");
        shipScroller = inventoryVisualElement.Q<ListView>("ship-list");

        itemButton = inventoryVisualElement.Q<VisualElement>("item-button");
        shipButton = inventoryVisualElement.Q<VisualElement>("ship-button");

        inventoryVisualElement.style.display = DisplayStyle.None;
    }

    private void Awake()
    {
        refreshListener = new UnityAction(DisplayInventoryEvent);
    }

    private void OnEnable()
    {
        EventManager.StartListening("storage UI Refresh", refreshListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening("storage UI Refresh", refreshListener);
    }

    public override VisualElement ReturnPage()
    {
        return inventoryVisualElement;
    }

    public override void SetCallbacks()
    {
        inventoryVisualElement.RegisterCallback<PointerUpEvent>(DropItemEvent);
        shipButton.RegisterCallback<ClickEvent>(ShipButtonPressed);
        itemButton.RegisterCallback<ClickEvent>(ItemButtonPressed);
    }

    public void DisplayInventoryEvent()
    {
        DisplayInventory(true);
    }

    public void ShowCorrectDisplay()
    {
        if (storageItems)
        {
            inventoryScroller.style.display = DisplayStyle.Flex;
            shipScroller.style.display = DisplayStyle.None;
        }
        else
        {
            inventoryScroller.style.display = DisplayStyle.None;
            shipScroller.style.display = DisplayStyle.Flex;
        }
    }

    public void SwitchShip(int i)
    {
        InstantiatedShip selectedShip = uiComponent.currentStation.shipStorage[i];
        Fleet playerFleet = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().fleetAssociation;
        GameObject originalPlayer = VulturaInstance.currentPlayer;

        uiComponent.currentStation.shipStorage.RemoveAt(i);
        uiComponent.currentStation.shipStorage.Add(originalPlayer.GetComponent<PrefabHandler>().currShip);
        VulturaInstance.RemoveFromSystem(originalPlayer.GetComponent<PrefabHandler>().currShip);

        Game.Instance.DestroyGameObject(originalPlayer);

        GameObject instantiatedShip = Game.Instance.InstantiateGameObject(selectedShip.ShipReference);
        instantiatedShip.GetComponent<PrefabHandler>().InitialPlayer();
        instantiatedShip.GetComponent<PrefabHandler>().currShip = selectedShip;

        VulturaInstance.AddSelectableToSystem(instantiatedShip.GetComponent<PrefabHandler>().currShip);


        playerFleet.FleetCommander = instantiatedShip.GetComponent<PrefabHandler>().currShip;
        VulturaInstance.currentPlayer = instantiatedShip;

        instantiatedShip.GetComponent<PrefabHandler>().currShip.selectableObject = VulturaInstance.currentPlayer;

        instantiatedShip.GetComponent<PrefabHandler>().fleetAssociation = playerFleet;

        shipScroller.Rebuild();
    }

    public void DisplayInventory(bool refresh = false)
    {
        Func<VisualElement> makeItemShip = () => uiComponent.loadableAssets["market-item"].Instantiate();

        Action<VisualElement, int> bindItemShip = (e, i) => {
            e.RegisterCallback<PointerEnterEvent>(ev => {
                (ev.target as VisualElement).Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));

                Debug.Log("Hovered");
            });

            e.RegisterCallback<PointerLeaveEvent>(ev => {
                (ev.target as VisualElement).Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
            });

            e.Q<VisualElement>("item-percentage").style.display = DisplayStyle.None;
            e.Q<VisualElement>("item-price").style.display = DisplayStyle.None;

            e.Q<Label>("item-name").text = uiComponent.currentStation.shipStorage[i].shipStats.name;

            e.RegisterCallback<ClickEvent>(ev => {
                SwitchShip(i);
            });
        };

        shipScroller.makeItem = makeItemShip;
        shipScroller.bindItem = bindItemShip;
        shipScroller.itemsSource = uiComponent.currentStation.shipStorage;

        ShowCorrectDisplay();
        if (!refresh)
        {
            if (inventoryVisualElement.style.display == DisplayStyle.Flex)
                inventoryVisualElement.style.display = DisplayStyle.None;
            else
                inventoryVisualElement.style.display = DisplayStyle.Flex;
        }


        if (storageItems)
        {
            Inventory stationInventory = uiComponent.currentStation.storage;

            inventoryScroller.Clear();

            for (int i = 0; i < stationInventory.itemList.Count; i++)
            {
                VisualElement tempItem = uiComponent.loadableAssets["inventory-item"].Instantiate();

                tempItem.style.width = Length.Percent(30f);
                tempItem.style.height = Length.Percent(25f);

                tempItem.style.marginLeft = 10f;
                tempItem.style.marginRight = 0f;
                tempItem.style.marginTop = 10f;
                tempItem.style.marginBottom = 0f;

                tempItem.Q<Label>("item-name").text = stationInventory.itemList[i].item.Name;
                tempItem.Q<Label>("item-count").text = stationInventory.itemList[i].quantity.ToString();

                tempItem.userData = new DragData(i, "station", uiComponent.currentStation.storage);

                inventoryScroller.Add(tempItem);

                tempItem.RegisterCallback<PointerDownEvent>(ev => {
                
                    VisualElement eventTarget = (ev.currentTarget as VisualElement);
                    MasterOSManager.Instance.currentDraggedElement = eventTarget;
                    MasterOSManager.Instance.currentDraggedElement.Q<VisualElement>("inventory-item").visible = false;

                    Debug.Log(MasterOSManager.Instance.visualDragger.Q<Label>("item-count"));

                    MasterOSManager.Instance.visualDragger.Q<Label>("item-count").text = eventTarget.Q<Label>("item-count").text;
                    MasterOSManager.Instance.visualDragger.Q<Label>("item-name").text = eventTarget.Q<Label>("item-name").text;

                    MasterOSManager.Instance.visualDragger.pickingMode = PickingMode.Ignore;
                    MasterOSManager.Instance.visualDragger.Q<VisualElement>("inventory-item").pickingMode = PickingMode.Ignore;              
                    MasterOSManager.Instance.visualDragger.style.position = Position.Absolute;
                    MasterOSManager.Instance.visualDragger.style.visibility = Visibility.Visible;
                    MasterOSManager.Instance.visualDragger.style.height = eventTarget.resolvedStyle.height;
                    MasterOSManager.Instance.visualDragger.style.width = eventTarget.resolvedStyle.width;
                    MasterOSManager.Instance.visualDragger.style.top = ev.position.y - eventTarget.resolvedStyle.height / 2;
                    MasterOSManager.Instance.visualDragger.style.left = ev.position.x - eventTarget.resolvedStyle.width / 2;
                    MasterOSManager.Instance.isDragging = true;

                    if (tempTooltip != null)
                    {
                        try 
                        {
                            MasterOSManager.Instance.rootVisualElement.Remove(tempTooltip);
                            tempTooltip = null;
                        } catch (System.ArgumentException ex)
                        {

                        }

                    }

                }); 

                tempItem.RegisterCallback<PointerEnterEvent>(ev => {
                    if (!MasterOSManager.Instance.isDragging)
                    {
                        if (tempTooltip == null)
                        {
                            tempTooltip = uiComponent.loadableAssets["item-tooltip"].Instantiate();
                        }

                        Color32 rarityColor = VulturaInstance.GenerateItemColor(stationInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item.Rarity);
                        tempTooltip.Q<VisualElement>("item-tooltip").style.borderBottomColor = new StyleColor(rarityColor);
                        tempTooltip.Q<VisualElement>("item-tooltip").style.borderLeftColor = new StyleColor(rarityColor);
                        tempTooltip.Q<VisualElement>("item-tooltip").style.borderRightColor = new StyleColor(rarityColor);
                        tempTooltip.Q<VisualElement>("item-tooltip").style.borderTopColor = new StyleColor(rarityColor);
                        tempTooltip.Q<Label>("item-name").text = stationInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item.Name;
                        tempTooltip.Q<Label>("item-name").style.color = new StyleColor(rarityColor);
                        tempTooltip.Q<Label>("item-category").text = stationInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item.Category;
                        tempTooltip.Q<Label>("item-rarity").text = VulturaInstance.enumStringParser(stationInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item.Rarity.ToString());
                        tempTooltip.Q<Label>("item-rarity").style.color = new StyleColor(rarityColor);

                        BaseItem currItem = stationInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item;

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

                                VisualElement statLabel = uiComponent.loadableAssets["item-stat"].Instantiate();
                                statLabel.Q<Label>("item-stat").text = mainStat.ReturnStatDescription();
                                mainStatElement.Add(statLabel);
                            }

                            foreach (ItemStat prefixStat in itemStatHandler.Prefixes)
                            {
                                VisualElement statLabel = uiComponent.loadableAssets["item-stat"].Instantiate();
                                statLabel.Q<Label>("item-stat").text = prefixStat.ReturnStatDescription();
                                affixesElement.Add(statLabel);
                            }

                            foreach (ItemStat suffixStat in itemStatHandler.Suffixes)
                            {
                                VisualElement statLabel = uiComponent.loadableAssets["item-stat"].Instantiate();
                                statLabel.Q<Label>("item-stat").text = suffixStat.ReturnStatDescription();
                                affixesElement.Add(statLabel);
                            }
                        }

                        tempTooltip.pickingMode = PickingMode.Ignore;
                        tempTooltip.style.position = Position.Absolute;
                        tempTooltip.style.top = ev.position.y;
                        tempTooltip.style.left = ev.position.x;
                        MasterOSManager.Instance.rootVisualElement.Add(tempTooltip);
                    }
                });

                tempItem.RegisterCallback<PointerMoveEvent>(ev => {
                    if (tempTooltip != null && !MasterOSManager.Instance.isDragging)
                    {
                        tempTooltip.style.top = ev.position.y;
                        tempTooltip.style.left = ev.position.x;
                    }
                });

                tempItem.RegisterCallback<PointerLeaveEvent>(ev => {
                    if (tempTooltip != null)
                    {
                        MasterOSManager.Instance.rootVisualElement.Remove(tempTooltip);
                        tempTooltip = null;
                    }
                });
            }
        }
        else
        {
            shipScroller.Rebuild();
        }

    }

    private void DropItemEvent(PointerUpEvent ev)
    {
        if (MasterOSManager.Instance.isDragging && (MasterOSManager.Instance.currentDraggedElement.userData as DragData).WindowContext != "station")
        {
            Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
            Inventory stationInventory = uiComponent.currentStation.storage;

            InventoryItem item = playerInventory.Pop((MasterOSManager.Instance.currentDraggedElement.userData as DragData).Index);
            
            stationInventory.Add(item, null);

            DisplayInventory(true);
            EventManager.TriggerEvent("inventory UI Refresh");
        }
    }

    private void ShipButtonPressed(ClickEvent ev)
    {
        storageItems = false;
        DisplayInventory(true);
    }

    private void ItemButtonPressed(ClickEvent ev)
    {
        storageItems = true;
        DisplayInventory(true);
    }
}
