using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public VisualTreeAsset inventoryRow;
    public VisualTreeAsset inventoryItem;

    VisualElement rootVisualElement;
    ScrollView inventoryScroller;

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

    }

    public void HandleInventory()
    {
        UIDocument inventoryDocument = GetComponent<UIDocument>();

        if (inventoryDocument.enabled)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        GetComponent<UIDocument>().enabled = true;
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        inventoryScroller = rootVisualElement.Q<ScrollView>("inventory-scroller");
        VisualElement row = inventoryRow.Instantiate();

        inventoryScroller.Add(row);
        VisualElement item = inventoryItem.Instantiate();
        item.style.width = Length.Percent(16.6f);
        row.Add(item);
    }

    public void CloseInventory()
    {
        GetComponent<UIDocument>().enabled = false;
    }
}
