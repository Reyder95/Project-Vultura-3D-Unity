using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class MiningStationUI : MonoBehaviour
{

    // Visual tree assets, to be instantiated when needed
    public VisualTreeAsset contactCard;
    public VisualTreeAsset inventoryRow;
    public VisualTreeAsset marketRow;

    // Game objects for the different station windows
    public GameObject homeGameobject;
    public GameObject contactGameobject;
    public GameObject marketGameobject;

    // Roots for each window
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

    // Market stuff
    public ListView marketList;
    public SliderInt quantitySlider;
    public VisualElement currentSelected;

    // The station of this UI
    public BaseStation station;

    // Additional variables
    public int selectedIndex = -1;  // The selected market index
    public int currQuantity = 1;    // The current quantity selected of the particular market item
    public bool inSpecify = false;  // State which determines whether or not a user is in the "specify" mode for moving an item between storage and inventory
    public Inventory playerInventory;


    // On enable, start every game object as inactive, and activate them when needed
    void OnEnable()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(false);
    }

    // When exiting the station, set every game object to false
    public void Exit()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(false);
    }

    // When entering a station
    public void OpenUI(BaseStation stationObject)
    {
        station = stationObject;    // Retrieve and set the current station
        playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;   // Set the player inventory variable to the current player ship's cargo
        InitializeHome();   // Initialize the homepage and display it

    }

    // Initialize the homepage and display it
    public void InitializeHome()
    {
        // Set the home game object to true, and the rest to false
        homeGameobject.SetActive(true);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(false);

        inSpecify = false;  // Since the homescreen is where we use this variable for station storage, we set it initially to false.

        homeRoot = homeGameobject.GetComponent<UIDocument>().rootVisualElement;     // Set the root visual element of the homepage

        // Initialize a lot more visual elements underneath the home root.
        storagePane = homeRoot.Q<VisualElement>("storage-element");
        shipPane = homeRoot.Q<VisualElement>("ship-storage");
        inventoryList = homeRoot.Q<ListView>("inventory-list");
        storageList = homeRoot.Q<ListView>("storage-list");
        shipList = homeRoot.Q<ListView>("ship-list");
        inventorySplit = homeRoot.Q<VisualElement>("item-transfer");
        storageSplit = homeRoot.Q<VisualElement>("item-transfer-storage");

        // Set the side panes to be hidden
        storagePane.style.display = DisplayStyle.None;
        shipPane.style.display = DisplayStyle.None;

        // Make the ship item display in the listview (will need adjustment for final UI)
        Func<VisualElement> makeItemShip = () => new Label();

        // For the ship listview, each item gets displayed as such
        Action<VisualElement, int> bindItemShip = (e, i) => {
            (e as Label).text = station.shipStorage[i].ShipStats.name;  // Set the label to the name of the ship

            // When the ship is clicked, run through the following code.
            e.RegisterCallback<ClickEvent>(ev => {
                // Initialize initial variables to prepare for switching the ship
                InstantiatedShip selectedShip = station.shipStorage[i];     // Grab the proper ship out of storage
                Fleet playerFleet = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().fleetAssociation;       // Grab the fleet associated with the current player's ship
                GameObject originalPlayer = VulturaInstance.currentPlayer;  // Hold the reference to the original "current player"
                
                station.shipStorage.RemoveAt(i);        // Remove the selected ship from the storage
                station.shipStorage.Add(originalPlayer.GetComponent<PrefabHandler>().currShip);     // Add the current player ship to the station's storage
                VulturaInstance.RemoveFromSystem(originalPlayer.GetComponent<PrefabHandler>().currShip);        // Remove the player's ship from the current system, as we are putting it in storage and removing it from the environment

                Destroy(originalPlayer);        // Destroy the player from the system

                GameObject instantiatedShip = Instantiate(selectedShip.ShipReference, VulturaInstance.currentPlayer.transform.position, VulturaInstance.currentPlayer.transform.rotation);
                instantiatedShip.GetComponent<PrefabHandler>().InitialPlayer();
                instantiatedShip.GetComponent<PrefabHandler>().currShip = selectedShip;

                VulturaInstance.AddToSystem(instantiatedShip.GetComponent<PrefabHandler>().currShip);

                playerFleet.FleetCommander = instantiatedShip.GetComponent<PrefabHandler>().currShip;
                VulturaInstance.currentPlayer = instantiatedShip;
                playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

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
            itemName.text = playerInventory.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = playerInventory.itemList[i].quantity.ToString();

            e.RegisterCallback<PointerDownEvent>(ev => {
                if (Input.GetKey("left shift"))
                {
                    if (!inSpecify)
                        VulturaInstance.SwapInventory(i, playerInventory, station.storage, VulturaInstance.MoveType.ALL);
                }
                else if (Input.GetKey("left ctrl"))
                {
                    inSpecify = true;
                    inventorySplit.style.top = ev.position.y - inventorySplit.layout.height;
                    inventorySplit.style.left = ev.position.x;
                    inventorySplit.Q<Label>("item-name").text = playerInventory.itemList[i].item.Name;
                    Label transferAmount = inventorySplit.Q<Label>("transfer-amount");
                    transferAmount.text = "1";
                    SliderInt swapSlider = inventorySplit.Q<SliderInt>("transfer-slider");
                    swapSlider.highValue = playerInventory.itemList[i].quantity;
                    swapSlider.lowValue = 1;
                    swapSlider.value = 1;
                    
                    swapSlider.RegisterValueChangedCallback(ev => {
                        transferAmount.text = ev.newValue.ToString();
                    });

                    inventorySplit.Q<Button>("ok-button").RegisterCallback<ClickEvent>(ev => {
                        VulturaInstance.SwapInventory(i, playerInventory, station.storage, VulturaInstance.MoveType.SPECIFY, swapSlider.value);
                        inSpecify = false;

                        inventoryList.Rebuild();
                        storageList.Rebuild();

                        inventorySplit.style.visibility = Visibility.Hidden;
                    });

                    inventorySplit.Q<Button>("cancel-button").RegisterCallback<ClickEvent>(ev => {
                        inSpecify = false;
                        inventorySplit.style.visibility = Visibility.Hidden;
                    });

                    inventorySplit.style.visibility = Visibility.Visible;   
                }
                else
                {
                    if(!inSpecify)
                        VulturaInstance.SwapInventory(i, playerInventory, station.storage, VulturaInstance.MoveType.SINGLE);
                }

                inventoryList.Rebuild();
                storageList.Rebuild();
                
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
                        VulturaInstance.SwapInventory(i, station.storage, playerInventory, VulturaInstance.MoveType.ALL);
                }
                else if (Input.GetKey("left ctrl"))
                {
                    inSpecify = true;
                    storageSplit.style.top = ev.position.y - storageSplit.layout.height;
                    storageSplit.style.left = ev.position.x;
                    storageSplit.Q<Label>("item-name").text = playerInventory.itemList[i].item.Name;
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
                        VulturaInstance.SwapInventory(i, station.storage, playerInventory, VulturaInstance.MoveType.SPECIFY, swapSlider.value);
                        inSpecify = false;

                        inventoryList.Rebuild();
                        storageList.Rebuild();

                        storageSplit.style.visibility = Visibility.Hidden;
                    });

                    storageSplit.Q<Button>("cancel-button").RegisterCallback<ClickEvent>(ev => {
                        inSpecify = false;

                        storageSplit.style.visibility = Visibility.Hidden;
                    });

                    storageSplit.style.visibility = Visibility.Visible;   
                }
                else
                {
                    if (!inSpecify)
                        VulturaInstance.SwapInventory(i, station.storage, playerInventory, VulturaInstance.MoveType.SINGLE);
                }

                inventoryList.Rebuild();
                storageList.Rebuild();
            });
        };

        inventoryList.makeItem = makeItemInventory;
        inventoryList.bindItem = bindItemInventory;
        inventoryList.itemsSource = playerInventory.itemList;

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
                    
                    InventoryItem itemInInventory = playerInventory.FindItem(station.market.itemList[i].item);

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

                        quantitySlider.highValue = itemInInventory.quantity;
                    });
                }
                
  
            });
        };

        Func<VisualElement> makeItemInventory = () => inventoryRow.Instantiate();
        Action<VisualElement, int> bindItemInventory = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = playerInventory.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = playerInventory.itemList[i].quantity.ToString();

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
        inventoryList.itemsSource = playerInventory.itemList;

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

            int itemIndex = playerInventory.FindItemIndex(station.market.itemList[selectedIndex].item);

            if (itemIndex != -1)
            {
                playerInventory.PopAmount(itemIndex, currQuantity);
                VulturaInstance.playerMoney += totalSellCost;

                inventoryList.Rebuild();
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
