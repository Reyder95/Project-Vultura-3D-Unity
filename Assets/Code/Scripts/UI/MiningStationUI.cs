using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MiningStationUI : MonoBehaviour
{
    public VisualTreeAsset contactCard;
    public VisualTreeAsset inventoryRow;

    public GameObject homeGameobject;
    public GameObject contactGameobject;

    public VisualElement homeRoot;
    public VisualElement contactRoot;

    // Storage stuff
    public VisualElement storagePane;
    public ListView inventoryList;
    public ListView storageList;

    public BaseStation station;

    void OnEnable()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
    }

    public void Exit()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
    }

    public void OpenUI(BaseStation stationObject)
    {
        station = stationObject;
        InitializeHome();

    }

    public void InventoryToStorage(int index)
    {
        InventoryItem inventoryItem = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Pop(index);
        station.storage.Add(inventoryItem);
        inventoryList.Rebuild();
        storageList.Rebuild();
    
    }

    public void StorageToInventory(int index)
    {
        InventoryItem inventoryItem = station.storage.Pop(index);
        VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Add(inventoryItem);
        inventoryList.Rebuild();
        storageList.Rebuild();
    }

    public void InitializeHome()
    {
        homeGameobject.SetActive(true);
        contactGameobject.SetActive(false);

        homeRoot = homeGameobject.GetComponent<UIDocument>().rootVisualElement;
        storagePane = homeRoot.Q<VisualElement>("storage-element");
        inventoryList = homeRoot.Q<ListView>("inventory-list");
        storageList = homeRoot.Q<ListView>("storage-list");

        storagePane.style.display = DisplayStyle.None;

        Func<VisualElement> makeItemInventory = () => inventoryRow.Instantiate();
        Action<VisualElement, int> bindItemInventory = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].quantity.ToString();

            e.RegisterCallback<ClickEvent>(ev => {
                InventoryToStorage(i);
            });
        };

        Action<VisualElement, int> bindItemStorage = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = station.storage.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = station.storage.itemList[i].quantity.ToString();

            e.RegisterCallback<ClickEvent>(ev => {
                StorageToInventory(i);
            });
        };

        inventoryList.makeItem = makeItemInventory;
        inventoryList.bindItem = bindItemInventory;
        inventoryList.itemsSource = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList;

        storageList.makeItem = makeItemInventory;
        storageList.bindItem = bindItemStorage;
        storageList.itemsSource = station.storage.itemList;


        homeRoot.Q<Button>("button-exit").RegisterCallback<ClickEvent>(ev => { Exit();});
        homeRoot.Q<Button>("button-contacts").RegisterCallback<ClickEvent>(ev => {
            InitializeContacts();
        });
        homeRoot.Q<Button>("storage-open").RegisterCallback<ClickEvent>(ev => {
            
            if (storagePane.style.display == DisplayStyle.None)
                storagePane.style.display = DisplayStyle.Flex;
            else
                storagePane.style.display = DisplayStyle.None;
        });
        homeRoot.Q<Label>("station-name").text = station.SelectableName;
    }

    public void InitializeContacts()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(true);
        contactRoot = contactGameobject.GetComponent<UIDocument>().rootVisualElement;
        contactRoot.Q<Label>("station-name").text = station.SelectableName;

        VisualElement contactVisual = contactRoot.Q<VisualElement>("contact-list");

        contactRoot.Q<Button>("button-back").RegisterCallback<ClickEvent>(ev => {
            InitializeHome();
        });

        var stationHeadInstance = contactCard.Instantiate();
        stationHeadInstance.Q<Label>("contact-name").text = station.stationHead.Name;
        stationHeadInstance.Q<Label>("contact-type").text = VulturaInstance.enumStringParser(station.stationHead.Type.ToString());
        contactVisual.Add(stationHeadInstance);

        foreach (Contact contactObject in station.contacts)
        {
            var contactInstance = contactCard.Instantiate();
            contactInstance.Q<Label>("contact-name").text = contactObject.Name;
            contactInstance.Q<Label>("contact-type").text = VulturaInstance.enumStringParser(contactObject.Type.ToString());
            contactVisual.Add(contactInstance);
        }
    }
}
