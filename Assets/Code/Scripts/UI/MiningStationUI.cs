using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MiningStationUI : MonoBehaviour
{
    public enum MoveType {
        SINGLE,
        ALL,
        SPECIFY
    }

    public VisualTreeAsset contactCard;
    public VisualTreeAsset inventoryRow;
    public VisualTreeAsset marketRow;

    public GameObject homeGameobject;
    public GameObject contactGameobject;
    public GameObject marketGameobject;

    public VisualElement homeRoot;
    public VisualElement contactRoot;
    public VisualElement marketRoot;

    // Storage stuff
    public VisualElement storagePane;
    public VisualElement shipPane;
    public ListView inventoryList;
    public ListView storageList;
    public ListView shipList;
    public VisualElement inventorySplit;
    public VisualElement storageSplit;

    public int selectedIndex = -1;
    public int currQuantity = 1;

    // Market stuff
    public ListView marketList;
    public SliderInt quantitySlider;
    public VisualElement currentSelected;

    public BaseStation station;

    public bool inSpecify = false;

    void OnEnable()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(false);
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

    public void InventoryToStorage(int index, MoveType moveType, int quantity = 0)
    {
        Inventory playerCargo = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
        InventoryItem inventoryItem = null;

        if (moveType == MoveType.SINGLE)
        {
            inventoryItem = playerCargo.PopAmount(index, 1);
        }
        else if (moveType == MoveType.ALL)
        {
            inventoryItem = playerCargo.Pop(index);
        }
        else if (moveType == MoveType.SPECIFY)
        {
            inventoryItem = playerCargo.PopAmount(index, quantity);
        }

        if (inventoryItem != null)
            station.storage.Add(inventoryItem);

        // InventoryItem inventoryItem = station.storage.Pop(index);
        // VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Add(inventoryItem);
        inventoryList.Rebuild();
        storageList.Rebuild();
    
    }

    public void StorageToInventory(int index, MoveType moveType, int quantity = 0)
    {
        Inventory playerCargo = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
        InventoryItem inventoryItem = null;

        if (moveType == MoveType.SINGLE)
        {
            inventoryItem = station.storage.PopAmount(index, 1);
        }
        else if (moveType == MoveType.ALL)
        {
            inventoryItem = station.storage.Pop(index);
        }
        else if (moveType == MoveType.SPECIFY)
        {
            inventoryItem = station.storage.PopAmount(index, quantity);
        }

        if (inventoryItem != null)
            playerCargo.Add(inventoryItem);

        // InventoryItem inventoryItem = station.storage.Pop(index);
        // VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Add(inventoryItem);
        inventoryList.Rebuild();
        storageList.Rebuild();
    }

    public void InitializeHome()
    {
        homeGameobject.SetActive(true);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(false);

        inSpecify = false;

        homeRoot = homeGameobject.GetComponent<UIDocument>().rootVisualElement;
        storagePane = homeRoot.Q<VisualElement>("storage-element");
        shipPane = homeRoot.Q<VisualElement>("ship-storage");
        inventoryList = homeRoot.Q<ListView>("inventory-list");
        storageList = homeRoot.Q<ListView>("storage-list");
        shipList = homeRoot.Q<ListView>("ship-list");
        inventorySplit = homeRoot.Q<VisualElement>("item-transfer");
        storageSplit = homeRoot.Q<VisualElement>("item-transfer-storage");

        storagePane.style.display = DisplayStyle.None;
        shipPane.style.display = DisplayStyle.None;

        Func<VisualElement> makeItemShip = () => new Label();
        Action<VisualElement, int> bindItemShip = (e, i) => {
            (e as Label).text = station.shipStorage[i].ShipStats.name;

            e.RegisterCallback<ClickEvent>(ev => {
                InstantiatedShip selectedShip = station.shipStorage[i];
                Quaternion shipQuaternion = VulturaInstance.currentPlayer.transform.rotation;
                Vector3 shipPosition = VulturaInstance.currentPlayer.transform.position;
                Fleet playerFleet = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().fleetAssociation;
                GameObject newShip = selectedShip.ShipReference;
                GameObject originalPlayer = VulturaInstance.currentPlayer;
                station.shipStorage.RemoveAt(i);
                station.shipStorage.Add(originalPlayer.GetComponent<PrefabHandler>().currShip);
                VulturaInstance.RemoveFromSystem(originalPlayer.GetComponent<PrefabHandler>().currShip);
                Destroy(originalPlayer);
                GameObject instantiatedShip = Instantiate(newShip, shipPosition, shipQuaternion);
                instantiatedShip.GetComponent<PrefabHandler>().InitialPlayer();
                instantiatedShip.GetComponent<PrefabHandler>().currShip = selectedShip;
                VulturaInstance.AddToSystem(instantiatedShip.GetComponent<PrefabHandler>().currShip);
                playerFleet.FleetCommander = instantiatedShip.GetComponent<PrefabHandler>().currShip;
                VulturaInstance.currentPlayer = instantiatedShip;
                instantiatedShip.GetComponent<PrefabHandler>().currShip.selectableObject = VulturaInstance.currentPlayer;
                instantiatedShip.GetComponent<PrefabHandler>().fleetAssociation = playerFleet;
                inventoryList.itemsSource = instantiatedShip.GetComponent<PrefabHandler>().currShip.Cargo.itemList;
                shipList.Rebuild();
                inventoryList.Rebuild();
                storageList.Rebuild();
                //station.SwitchShip(i);
            });
        };

        Func<VisualElement> makeItemInventory = () => inventoryRow.Instantiate();
        Action<VisualElement, int> bindItemInventory = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].quantity.ToString();

            e.RegisterCallback<PointerDownEvent>(ev => {
                if (Input.GetKey("left shift"))
                {
                    if (!inSpecify)
                        InventoryToStorage(i, MoveType.ALL);
                }
                else if (Input.GetKey("left ctrl"))
                {
                    inSpecify = true;
                    inventorySplit.style.top = ev.position.y - inventorySplit.layout.height;
                    inventorySplit.style.left = ev.position.x;
                    inventorySplit.Q<Label>("item-name").text = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].item.Name;
                    Label transferAmount = inventorySplit.Q<Label>("transfer-amount");
                    transferAmount.text = "1";
                    SliderInt swapSlider = inventorySplit.Q<SliderInt>("transfer-slider");
                    swapSlider.highValue = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].quantity;
                    swapSlider.lowValue = 1;
                    swapSlider.value = 1;
                    
                    swapSlider.RegisterValueChangedCallback(ev => {
                        transferAmount.text = ev.newValue.ToString();
                    });

                    inventorySplit.Q<Button>("ok-button").RegisterCallback<ClickEvent>(ev => {
                        InventoryToStorage(i, MoveType.SPECIFY, swapSlider.value);
                        inSpecify = false;

                        inventorySplit.style.visibility = Visibility.Hidden;
                    });

                    inventorySplit.Q<Button>("cancel-button").RegisterCallback<ClickEvent>(ev => {
                        inventorySplit.style.visibility = Visibility.Hidden;
                    });

                    inventorySplit.style.visibility = Visibility.Visible;   
                }
                else
                {
                    if(!inSpecify)
                        InventoryToStorage(i, MoveType.SINGLE);
                }
                
            });
        };

        Action<VisualElement, int> bindItemStorage = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = station.storage.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = station.storage.itemList[i].quantity.ToString();

            e.RegisterCallback<PointerDownEvent>(ev => {
                if (Input.GetKey("left shift"))
                {
                    if (!inSpecify)
                        StorageToInventory(i, MoveType.ALL);
                }
                else if (Input.GetKey("left ctrl"))
                {
                    inSpecify = true;
                    storageSplit.style.top = ev.position.y - storageSplit.layout.height;
                    storageSplit.style.left = ev.position.x;
                    storageSplit.Q<Label>("item-name").text = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].item.Name;
                    Label transferAmount = storageSplit.Q<Label>("transfer-amount");
                    transferAmount.text = "1";
                    SliderInt swapSlider = storageSplit.Q<SliderInt>("transfer-slider");
                    swapSlider.highValue = station.storage.itemList[i].quantity;
                    swapSlider.lowValue = 1;
                    swapSlider.value = 1;
                    
                    swapSlider.RegisterValueChangedCallback(ev => {
                        transferAmount.text = ev.newValue.ToString();
                    });

                    storageSplit.Q<Button>("ok-button").RegisterCallback<ClickEvent>(ev => {
                        StorageToInventory(i, MoveType.SPECIFY, swapSlider.value);
                        inSpecify = false;

                        storageSplit.style.visibility = Visibility.Hidden;
                    });

                    storageSplit.Q<Button>("cancel-button").RegisterCallback<ClickEvent>(ev => {
                        storageSplit.style.visibility = Visibility.Hidden;
                    });

                    storageSplit.style.visibility = Visibility.Visible;   
                }
                else
                {
                    if (!inSpecify)
                        StorageToInventory(i, MoveType.SINGLE);
                }
            });
        };

        inventoryList.makeItem = makeItemInventory;
        inventoryList.bindItem = bindItemInventory;
        inventoryList.itemsSource = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList;

        storageList.makeItem = makeItemInventory;
        storageList.bindItem = bindItemStorage;
        storageList.itemsSource = station.storage.itemList;

        shipList.makeItem = makeItemShip;
        shipList.bindItem = bindItemShip;
        shipList.itemsSource = station.shipStorage;


        homeRoot.Q<Button>("button-exit").RegisterCallback<ClickEvent>(ev => { Exit();});
        homeRoot.Q<Button>("button-contacts").RegisterCallback<ClickEvent>(ev => {
            InitializeContacts();
        });

        homeRoot.Q<Button>("button-market").RegisterCallback<ClickEvent>(ev => {
            InitializeMarket();
        });

        homeRoot.Q<Button>("storage-open").RegisterCallback<ClickEvent>(ev => {
            
            if (storagePane.style.display == DisplayStyle.None)
                storagePane.style.display = DisplayStyle.Flex;
            else
                storagePane.style.display = DisplayStyle.None;
        });
        homeRoot.Q<Button>("ship-storage-button").RegisterCallback<ClickEvent>(ev => {
            if (shipPane.style.display == DisplayStyle.None)
                shipPane.style.display = DisplayStyle.Flex;
            else
                shipPane.style.display = DisplayStyle.None;
        });
        homeRoot.Q<Label>("station-name").text = station.SelectableName;
    }

    public void InitializeContacts()
    {
        homeGameobject.SetActive(false);
        marketGameobject.SetActive(false);
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

    public void InitializeMarket()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(true);

        marketRoot = marketGameobject.GetComponent<UIDocument>().rootVisualElement;

        inventoryList = marketRoot.Q<ListView>("inventory-list");
        storageList = marketRoot.Q<ListView>("storage-list");
        marketList = marketRoot.Q<ListView>("market-list");
        quantitySlider = marketRoot.Q<SliderInt>("bottom-slider");

        quantitySlider.RegisterValueChangedCallback(ev => {
            currQuantity = ev.newValue;
            marketRoot.Q<Label>("bottom-item-quantity").text = ev.newValue.ToString();
            marketRoot.Q<Label>("bottom-item-buy-price").text = "$" + (station.market.itemList[selectedIndex].buyPrice * currQuantity).ToString();
        });

        marketRoot.Q<VisualElement>("none-display").style.display = DisplayStyle.Flex;
        marketRoot.Q<VisualElement>("purchase-display").style.display = DisplayStyle.None;

        Func<VisualElement> makeItemMarket = () => marketRow.Instantiate();
        Action<VisualElement, int> bindItemMarket = (e, i) => {

            e.Q<VisualElement>("main-visual").EnableInClassList("non-active", true);
            e.Q<VisualElement>("main-visual").EnableInClassList("active", false);

            var itemName = e.Q<Label>("item-name");
            itemName.text = station.market.itemList[i].item.Name;

            var itemType = e.Q<Label>("item-type");
            itemType.text = VulturaInstance.enumStringParser(station.market.itemList[i].item.Type.ToString());

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = station.market.itemList[i].quantity.ToString();

            var itemBuy = e.Q<Label>("item-buy");
            itemBuy.text = "$" + station.market.itemList[i].buyPrice.ToString();

            var itemSell = e.Q<Label>("item-sell");
            itemSell.text = "$" + station.market.itemList[i].sellPrice.ToString();

            e.RegisterCallback<PointerDownEvent>(ev => {
            
                if (currentSelected != null)
                {
                    currentSelected.Q<VisualElement>("main-visual").EnableInClassList("non-active", true);
                    currentSelected.Q<VisualElement>("main-visual").EnableInClassList("active", false);
                }

                selectedIndex = i;

                currentSelected = e;

                e.Q<VisualElement>("main-visual").EnableInClassList("non-active", false);
                e.Q<VisualElement>("main-visual").EnableInClassList("active", true);

                marketRoot.Q<VisualElement>("none-display").style.display = DisplayStyle.None;
                marketRoot.Q<VisualElement>("purchase-display").style.display = DisplayStyle.Flex;

                marketRoot.Q<Label>("bottom-item-name").text = station.market.itemList[i].item.Name;
                
                
                Button buyButton = marketRoot.Q<Button>("buy-button");

                if (!station.market.itemList[i].sellOnly)
                {
                    quantitySlider.lowValue = 1;
                    quantitySlider.highValue = station.market.itemList[i].quantity;
                    quantitySlider.value = 1; 
                    
                    float currentPercentDeviation = Mathf.Abs(((float)station.market.itemList[i].item.GalacticPrice - (float)station.market.itemList[i].buyPrice) / (float)station.market.itemList[i].buyPrice * 100.0f);
                    marketRoot.Q<Label>("bottom-item-buy-price").text = "$" + station.market.itemList[i].buyPrice.ToString();
                    if (station.market.itemList[i].item.GalacticPrice > station.market.itemList[i].buyPrice)
                        marketRoot.Q<Label>("bottom-item-difference").text = "<color=\"green\">-" + currentPercentDeviation.ToString("N2") + "%</color>";
                    else
                        marketRoot.Q<Label>("bottom-item-difference").text = "<color=\"red\">+" + currentPercentDeviation.ToString("N2") + "%</color>";
                    buyButton.text = "Buy";
                    buyButton.RegisterCallback<ClickEvent>(ev => {
                        BuyItem();
                        //quantitySlider.highValue = station.market.itemList[selectedIndex].quantity;
                    });
                }
                else
                {
                    float currentPercentDeviation = Mathf.Abs(((float)station.market.itemList[i].item.GalacticPrice - (float)station.market.itemList[i].sellPrice) / (float)station.market.itemList[i].sellPrice * 100.0f);
                    buyButton.text = "Sell";
                    if (station.market.itemList[i].item.GalacticPrice > station.market.itemList[i].sellPrice)
                        marketRoot.Q<Label>("bottom-item-difference").text = "<color=\"red\">-" + currentPercentDeviation.ToString("N2") + "%</color>";
                    else
                        marketRoot.Q<Label>("bottom-item-difference").text = "<color=\"green\">+" + currentPercentDeviation.ToString("N2") + "%</color>";
                    marketRoot.Q<Label>("bottom-item-buy-price").text = "$" + station.market.itemList[i].sellPrice.ToString();
                    
                    InventoryItem itemInInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.FindItem(station.market.itemList[i].item);

                    if (itemInInventory != null)
                    {
                        quantitySlider.lowValue = 1;
                        quantitySlider.value = 1; 
                        quantitySlider.highValue = itemInInventory.quantity;
                    }
                    else
                    {
                        quantitySlider.lowValue = 0;
                    }

                    buyButton.RegisterCallback<ClickEvent>(ev => {
                        SellItem();
                    });
                }
                
  
            });
        };

        Func<VisualElement> makeItemInventory = () => inventoryRow.Instantiate();
        Action<VisualElement, int> bindItemInventory = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[i].quantity.ToString();

            e.RegisterCallback<ClickEvent>(ev => {
                
            });
        };

        Action<VisualElement, int> bindItemStorage = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = station.storage.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = station.storage.itemList[i].quantity.ToString();

            e.RegisterCallback<ClickEvent>(ev => {
                
            });
        };

        marketList.makeItem = makeItemMarket;
        marketList.bindItem = bindItemMarket;
        marketList.itemsSource = station.market.itemList;

        inventoryList.makeItem = makeItemInventory;
        inventoryList.bindItem = bindItemInventory;
        inventoryList.itemsSource = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList;

        storageList.makeItem = makeItemInventory;
        storageList.bindItem = bindItemStorage;
        storageList.itemsSource = station.storage.itemList;

        marketRoot.Q<Button>("back-button").RegisterCallback<ClickEvent>(ev => {
            InitializeHome();
        });
    }

    void SellItem()
    {
        int totalSellCost = station.market.itemList[selectedIndex].sellPrice * currQuantity;

        bool success = station.InsertIntoStockpile(station.market.itemList[selectedIndex].item, currQuantity);

        if (success)
        {
            Inventory cargo = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

            int itemIndex = cargo.FindItemIndex(station.market.itemList[selectedIndex].item);

            if (itemIndex != -1)
            {
                cargo.PopAmount(itemIndex, currQuantity);
                VulturaInstance.playerMoney += totalSellCost;
            }
            
        }
    }

    void BuyItem()
    {
        int totalCost = station.market.itemList[selectedIndex].buyPrice * currQuantity;

        if (VulturaInstance.playerMoney >= totalCost)
        {
            InventoryItem purchasedItem = station.market.Purchase(selectedIndex, currQuantity);
            
            if (purchasedItem != null)
            {
                Debug.Log(purchasedItem.item);
                bool successfulAdd = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.AddToCargo(purchasedItem);

                if (!successfulAdd)
                    station.storage.Add(purchasedItem);
                inventoryList.Rebuild();
                marketList.Rebuild();

                VulturaInstance.playerMoney -= totalCost;

            }
        }
    }
}
