using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class InventoryOS : BaseOS
{   
    private UnityAction initListener;
    private UnityAction openListener;
    private UnityAction refreshListener;
    public VisualTreeAsset inventoryItem;
    public VisualTreeAsset itemTooltip;
    public VisualTreeAsset itemTooltipStat;
    
    public VisualElement inventoryScroller;
    public VisualElement cargoBar;
    public Label cargoCapacity;
    public VisualElement tempTooltip;
    public List<VisualElement> items = new List<VisualElement>();

    public InventoryOS(string windowName, VisualElement screen): base(windowName, screen) {}

    public override void Awake()
    {

        Debug.Log("Awake test!");
        initListener = new UnityAction(InitializeScreen);
        openListener = new UnityAction(OpenScreen);
        refreshListener = new UnityAction(DisplayInventory);
    }

    public override void OnEnable()
    {
        EventManager.StartListening("inventory UI Event", initListener);
        EventManager.StartListening("inventory UI Open", openListener);
        EventManager.StartListening("inventory UI Refresh", refreshListener);
    }

    public override void OnDisable()
    {
        EventManager.StopListening("inventory UI Event", initListener);
        EventManager.StopListening("inventory UI Open", openListener);
        EventManager.StopListening("inventory UI Refresh", refreshListener);
    }

    public override void Update()
    {
        if (screen != null)
        {
            if (UIScreenManager.Instance.focusedScreen == screen)
            {
                screen.Q<VisualElement>("screen-background").style.opacity = 1.0f;
            }
            else
            {
                screen.Q<VisualElement>("screen-background").style.opacity = 0.2f;
            }
        }

    }

    public override void InitializeScreen()
    {
        windowName = "inventory";
        
        base.InitializeScreen();

        Debug.Log("Another test!!");

        screen.Q<VisualElement>("screen-background").RegisterCallback<PointerDownEvent>(ev => {
            UIScreenManager.Instance.SetFocusedScreen(screen);
        });

        inventoryScroller = screen.Q<VisualElement>("inventory-scroller").Q<VisualElement>("inventory-scroller");
        cargoBar = screen.Q<VisualElement>("bar-background");
        cargoCapacity = screen.Q<Label>("cargo-capacity");

        screen.Q<VisualElement>("exit-button").RegisterCallback<ClickEvent>(ev => {
            OpenScreen();
        });

        screen.RegisterCallback<PointerUpEvent>(DropItemEvent);
    }

    public override void OpenScreen()
    {
        base.OpenScreen();

        DisplayInventory();

        MasterOSManager.Instance.inventoryOpen = !MasterOSManager.Instance.inventoryOpen;

        if (MasterOSManager.Instance.inventoryOpen)
            screen.style.display = DisplayStyle.Flex;
        else
        {
            screen.style.display = DisplayStyle.None;

            try
            {
                MasterOSManager.Instance.rootVisualElement.Remove(tempTooltip);
                tempTooltip = null;
            }
            catch (System.ArgumentException ex)
            {
                Debug.Log("There's an argument exception.");
            }

        }
    }

    private void DisplayInventory()
    {
        try
        {
            inventoryScroller.Clear();
            Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
            MasterOSManager.Instance.visualDragger = MasterOSManager.Instance.rootVisualElement.Q<VisualElement>("ghost-item");

            for (int i = 0; i < playerInventory.itemList.Count; i++)
            {
                VisualElement item = loadableAssets["inventory-item"].Instantiate();

                item.style.width = Length.Percent(15f);
                item.style.height = Length.Percent(35f);

                item.style.marginLeft = 10f;
                item.style.marginRight = 0f;
                item.style.marginTop = 10f;
                item.style.marginBottom = 0f;

                item.Q<Label>("item-name").text = playerInventory.itemList[i].item.Name;
                item.Q<Label>("item-count").text = playerInventory.itemList[i].quantity.ToString();

                item.userData = new DragData(i, "inventory", playerInventory);

                MasterOSManager.Instance.DraggerEvents(item);          

                item.RegisterCallback<PointerEnterEvent>(ev => {
                    if (!MasterOSManager.Instance.isDragging)
                    {
                        if (tempTooltip == null)
                        {
                            tempTooltip = loadableAssets["item-tooltip"].Instantiate();
                        }

                        Color32 rarityColor = VulturaInstance.GenerateItemColor(playerInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item.Rarity);
                        tempTooltip.Q<VisualElement>("item-tooltip").style.borderBottomColor = new StyleColor(rarityColor);
                        tempTooltip.Q<VisualElement>("item-tooltip").style.borderLeftColor = new StyleColor(rarityColor);
                        tempTooltip.Q<VisualElement>("item-tooltip").style.borderRightColor = new StyleColor(rarityColor);
                        tempTooltip.Q<VisualElement>("item-tooltip").style.borderTopColor = new StyleColor(rarityColor);
                        tempTooltip.Q<Label>("item-name").text = playerInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item.Name;
                        tempTooltip.Q<Label>("item-name").style.color = new StyleColor(rarityColor);
                        tempTooltip.Q<Label>("item-category").text = playerInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item.Category;
                        tempTooltip.Q<Label>("item-rarity").text = VulturaInstance.enumStringParser(playerInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item.Rarity.ToString());
                        tempTooltip.Q<Label>("item-rarity").style.color = new StyleColor(rarityColor);

                        BaseItem currItem = playerInventory.itemList[((ev.currentTarget as VisualElement).userData as DragData).Index].item;

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

                                VisualElement statLabel = loadableAssets["item-stat"].Instantiate();
                                statLabel.Q<Label>("item-stat").text = mainStat.ReturnStatDescription();
                                mainStatElement.Add(statLabel);
                            }

                            foreach (ItemStat prefixStat in itemStatHandler.Prefixes)
                            {
                                VisualElement statLabel = loadableAssets["item-stat"].Instantiate();
                                statLabel.Q<Label>("item-stat").text = prefixStat.ReturnStatDescription();
                                affixesElement.Add(statLabel);
                            }

                            foreach (ItemStat suffixStat in itemStatHandler.Suffixes)
                            {
                                VisualElement statLabel = loadableAssets["item-stat"].Instantiate();
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
                    else
                    {
                        MasterOSManager.Instance.elementOver = (ev.currentTarget as VisualElement);
                    }
                });


                item.RegisterCallback<PointerMoveEvent>(ev => {
                    if (tempTooltip != null && !MasterOSManager.Instance.isDragging)
                    {
                        tempTooltip.style.top = ev.position.y;
                        tempTooltip.style.left = ev.position.x;
                    }
                });
                item.RegisterCallback<PointerLeaveEvent>(ev => {
                    if (tempTooltip != null)
                    {
                        MasterOSManager.Instance.rootVisualElement.Remove(tempTooltip);
                        tempTooltip = null;
                    }

                    MasterOSManager.Instance.elementOver = null;
                });

                inventoryScroller.Add(item);
            }

            float percentUsed = (playerInventory.currCargo / VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().shipStats.baseCargo) * 100;

            cargoCapacity.text = playerInventory.currCargo.ToString("N2") + " / " + VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().shipStats.baseCargo.ToString() + " (" + percentUsed.ToString("N1") + "%)";

            cargoBar.Q<VisualElement>("bar-percent").style.width = Length.Percent(percentUsed);



        } catch (System.NullReferenceException ex)
        {
            Debug.Log("Inventory isn't loaded yet!");
        }
    }

    public void InventoryEvent()
    {
        if (!MasterOSManager.Instance.isDragging)
        {
            try
            {
                if (tempTooltip != null)
                {
                    screen.Remove(tempTooltip);
                    tempTooltip = null; 
                }

                DisplayInventory(); 
    
            } catch (System.NullReferenceException ex)
            {
                Debug.Log("Inventory isn't loaded yet!");
            }
        }
    }

    private void DropItemEvent(PointerUpEvent ev)
    {
        if (MasterOSManager.Instance.isDragging && (MasterOSManager.Instance.currentDraggedElement.userData as DragData).WindowContext != "inventory")
        {
            Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
            Inventory stationInventory = (MasterOSManager.Instance.currentDraggedElement.userData as DragData).InvFrom;

            InventoryItem item = stationInventory.Pop((MasterOSManager.Instance.currentDraggedElement.userData as DragData).Index);
            
            bool success = playerInventory.Add(item, VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip);

            if (!success)
                stationInventory.Add(item, null);

            DisplayInventory();
            EventManager.TriggerEvent("storage UI Refresh");
        }
    }
}
