using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public VisualTreeAsset inventoryRow;
    public VisualTreeAsset inventoryItem;
    public VisualTreeAsset itemTooltip;

    bool isDragging = false;

    List<VisualElement> items = new List<VisualElement>();

    VisualElement rootVisualElement;
    ScrollView inventoryScroller;
    VisualElement inventory;

    VisualElement currentDraggedElement;
    VisualElement currentElementOver;

    VisualElement visualDragger;
    VisualElement tempTooltip;

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
    }

    void OnEnable()
    {
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;   
        inventoryScroller = rootVisualElement.Q<ScrollView>("inventory-scroller");
        inventory = rootVisualElement.Q<VisualElement>("base-content");
        inventory.RegisterCallback<MouseUpEvent>(ev => {
            print("Test!!!!!");
        });
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
            

            Debug.Log("SCOTLAND!!");
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

    public void OpenInventory()
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
                currentDraggedElement = item;
                currentDraggedElement.Q<VisualElement>("inventory-item").visible = false;

                visualDragger.Q<Label>("item-quantity").text = item.Q<Label>("item-quantity").text;
                visualDragger.Q<Label>("item-name").text = item.Q<Label>("item-name").text;

                visualDragger.pickingMode = PickingMode.Ignore;
                visualDragger.Q<VisualElement>("inventory-item").pickingMode = PickingMode.Ignore;              
                visualDragger.style.position = Position.Absolute;
                Vector3 pos = Input.mousePosition;
                visualDragger.Q<VisualElement>("inventory-item").style.visibility = Visibility.Visible;
                visualDragger.style.top = ev.position.y - visualDragger.layout.height / 2;
                visualDragger.style.left = ev.position.x - visualDragger.layout.width / 2;
                isDragging = true;

                visualDragger.RegisterCallback<PointerMoveEvent>(ev => {
                    if (!isDragging)
                        return;
                    visualDragger.style.top = ev.position.y - visualDragger.layout.height / 2;
                    visualDragger.style.left = ev.position.x - visualDragger.layout.width / 2;
                });

                visualDragger.RegisterCallback<PointerLeaveEvent>(ev => {
                    if (!isDragging)
                        return;
                    visualDragger.style.top = ev.position.y - visualDragger.layout.height / 2;
                    visualDragger.style.left = ev.position.x - visualDragger.layout.width / 2;
                });

                visualDragger.RegisterCallback<PointerUpEvent>(ev => {
                    visualDragger.Q<VisualElement>("inventory-item").style.visibility = Visibility.Hidden;
                });
            });
            item.RegisterCallback<PointerEnterEvent>(ev => {
                tempTooltip = itemTooltip.Instantiate();
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

            item.RegisterCallback<PointerUpEvent>(ev => {
                Debug.Log("Pointer up!");
            });

            items.Add(item);

            currentRow.Add(item);
        }

        float percentUsed = (playerInventory.currCargo / VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().shipStats.baseCargo) * 100;

        rootVisualElement.Q<Label>("cargo-capacity").text = playerInventory.currCargo.ToString() + " / " + VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().shipStats.baseCargo.ToString() + " (" + percentUsed.ToString("N1") + "%)";

        rootVisualElement.Q<VisualElement>("bar-percent").style.width = Length.Percent(percentUsed);

        rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void CloseInventory()
    {
        rootVisualElement.style.display = DisplayStyle.None;
    }
}
