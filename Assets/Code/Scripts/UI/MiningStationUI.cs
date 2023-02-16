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
    public VisualTreeAsset contractItem;
    public VisualTreeAsset currentContractItem;
    public VisualTreeAsset facilityItem;

    // Game objects for the different station windows
    public GameObject homeGameobject;
    public GameObject contactGameobject;
    public GameObject marketGameobject;
    public GameObject cargoGameobject;
    public GameObject facilityGameobject;

    // Roots for each window
    public VisualElement homeRoot;
    public VisualElement contactRoot;
    public VisualElement marketRoot;
    public VisualElement cargoRoot;
    public VisualElement facilityRoot;

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

    //Contact stuff
    public ConversationStack convoStack;
    public VisualElement selectedContact;

    // Cargo stuff
    public VisualElement selectedContract;

    // Facility stuff
    public Facility currentFacility;

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
        // Initially, set every single UI game object to inactive.
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(false);
        cargoGameobject.SetActive(false);
        facilityGameobject.SetActive(false);
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Exit();
    }

    // When exiting the station, set every game object to false
    public void Exit()
    {
        // Set all game objects to false when leaving the UI
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(false);
        cargoGameobject.SetActive(false);
        facilityGameobject.SetActive(false);
        VulturaInstance.playerStatus = VulturaInstance.PlayerStatus.SPACE;
    }

    // When entering a station
    public void OpenUI(BaseStation stationObject)
    {
        if (VulturaInstance.playerStatus != VulturaInstance.PlayerStatus.STATION)
        {
            station = stationObject;    // Retrieve and set the current station
            playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;   // Set the player inventory variable to the current player ship's cargo
            InitializeHome();   // Initialize the homepage and display it
            VulturaInstance.playerStatus = VulturaInstance.PlayerStatus.STATION;
        }
        

    }

    // Populate facilities on front homepage
    public void PopulateFacilities()
    {
        // For every facility in the station, create a facility icon with its own name.
        for (int i = 0; i < station.facilities.Count; i++)
        {
            VisualElement facilityInstance = facilityItem.Instantiate();
            facilityInstance.userData = station.facilities[i];
            facilityInstance.style.width = Length.Percent(16.6f);

            facilityInstance.Q<Label>("facility-name").text = station.facilities[i].facilityName;

            if (!station.facilities[i].demand)
            {
                facilityInstance.Q<Label>("facility-status").text = "<color=\"green\">OK</color>";
            }
            else
            {
                facilityInstance.Q<Label>("facility-status").text = "<color=\"red\">In Demand</color>";
            }

            facilityInstance.RegisterCallback<ClickEvent>(ev => {
                currentFacility = (ev.currentTarget as VisualElement).userData as Facility;
                InitializeFacility();
            });

            homeRoot.Q<VisualElement>("facility-inner").Add(facilityInstance);
        }
    }

    // Initialize the homepage and display it
    public void InitializeHome()
    {
        // Set the home game object to true, and the rest to false
        homeGameobject.SetActive(true);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(false);
        cargoGameobject.SetActive(false);
        facilityGameobject.SetActive(false);

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

        PopulateFacilities();

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
                SwitchShip(i);
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

        homeRoot.Q<Button>("button-contracts").RegisterCallback<ClickEvent>(ev => {
            InitializeCargo();
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

    // Switch the player's ship to the selected one.
    public void SwitchShip(int i)
    {
        // Initialize initial variables to prepare for switching the ship
        InstantiatedShip selectedShip = station.shipStorage[i];     // Grab the proper ship out of storage
        Fleet playerFleet = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().fleetAssociation;       // Grab the fleet associated with the current player's ship
        GameObject originalPlayer = VulturaInstance.currentPlayer;  // Hold the reference to the original "current player"
                
        station.shipStorage.RemoveAt(i);        // Remove the selected ship from the storage
        station.shipStorage.Add(originalPlayer.GetComponent<PrefabHandler>().currShip);     // Add the current player ship to the station's storage
        VulturaInstance.RemoveFromSystem(originalPlayer.GetComponent<PrefabHandler>().currShip);        // Remove the player's ship from the current system, as we are putting it in storage and removing it from the environment

        Destroy(originalPlayer);        // Destroy the player from the system

        // Instantiate the selected ship in the world at the location the player was previously, with the same rotation.s
        GameObject instantiatedShip = Instantiate(selectedShip.ShipReference, VulturaInstance.currentPlayer.transform.position, VulturaInstance.currentPlayer.transform.rotation);
        instantiatedShip.GetComponent<PrefabHandler>().InitialPlayer();
        instantiatedShip.GetComponent<PrefabHandler>().currShip = selectedShip;

        // Add the new ship to the system.
        VulturaInstance.AddToSystem(instantiatedShip.GetComponent<PrefabHandler>().currShip);

        // Switch the fleet commander (which would always be the player at this point) to the new player ship
        playerFleet.FleetCommander = instantiatedShip.GetComponent<PrefabHandler>().currShip;
        VulturaInstance.currentPlayer = instantiatedShip;

        // Set the playerInventory variable to the new ship's cargo
        playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

        // Set the selectable object of the instantiated ship to the current player's game object.
        instantiatedShip.GetComponent<PrefabHandler>().currShip.selectableObject = VulturaInstance.currentPlayer;

        // Set the player fleet's associated fleet in memory
        instantiatedShip.GetComponent<PrefabHandler>().fleetAssociation = playerFleet;

        // Set the item source of the inventory list to the current player's inventory (switching a ship means that needs to be reset)
        inventoryList.itemsSource = instantiatedShip.GetComponent<PrefabHandler>().currShip.Cargo.itemList;

        // Rebuild all the inventory lists and the ship list.
        shipList.Rebuild();
        inventoryList.Rebuild();
        storageList.Rebuild();
    }

    // Initialize the facility window
    public void InitializeFacility()
    {
        // Turn off every other game object besides the facility.
        homeGameobject.SetActive(false);
        marketGameobject.SetActive(false);
        contactGameobject.SetActive(false);
        cargoGameobject.SetActive(false);
        facilityGameobject.SetActive(true);

        // Set the facility root visual element
        facilityRoot = facilityGameobject.GetComponent<UIDocument>().rootVisualElement;

        // Set some basic text on the screen in preparation for the rest
        facilityRoot.Q<Label>("facility-name").text = currentFacility.facilityName + " - ";
        facilityRoot.Q<Label>("facility-production").text = "Producing ";

        // Write out every item this facility produces
        for (int i = 0; i < currentFacility.producing.Length; i++)
        {
            BaseItem item = currentFacility.producing[i].itemExec();
            if (i == 0)
                facilityRoot.Q<Label>("facility-production").text += item.Name;
            else if (i < i - 1 && currentFacility.producing.Length > 1)
                facilityRoot.Q<Label>("facility-production").text += ", " + item.Name;
            else
                facilityRoot.Q<Label>("facility-production").text += " and " + item.Name;
        }

        // Determine whether this facility is in demand. Write OK or In Demand depending
        if (currentFacility.demand)
        {
            facilityRoot.Q<Label>("facility-name").text += " <color=\"red\">In Demand</color>";
        }
        else
        {
            facilityRoot.Q<Label>("facility-name").text += " <color=\"green\">OK</color>";
        }

        // Set the back button to return to the home
        facilityRoot.Q<Button>("back-button").RegisterCallback<ClickEvent>(ev => {
            InitializeHome();
        });
    }

    // Go to the contacts page
    public void InitializeContacts()
    {
        // Set every game object inactive except the contacts game object
        homeGameobject.SetActive(false);
        marketGameobject.SetActive(false);
        contactGameobject.SetActive(true);
        cargoGameobject.SetActive(false);
        facilityGameobject.SetActive(false);

        // Set the contact root visual element
        contactRoot = contactGameobject.GetComponent<UIDocument>().rootVisualElement;
        contactRoot.Q<Label>("station-name").text = station.SelectableName;

        // Grab the contact container visual element
        VisualElement contactVisual = contactRoot.Q<VisualElement>("contact-list");

        // If the back button is pressed, go back home
        contactRoot.Q<Button>("button-back").RegisterCallback<ClickEvent>(ev => {
            InitializeHome();
        });

        // Instantiate and prepare the content of the station head card
        var stationHeadInstance = contactCard.Instantiate();
        stationHeadInstance.Q<Label>("contact-name").text = station.stationHead.Name;
        stationHeadInstance.Q<Label>("contact-type").text = VulturaInstance.enumStringParser(station.stationHead.Type.ToString());
        stationHeadInstance.userData = station.stationHead;
        stationHeadInstance.RegisterCallback<ClickEvent>(ev => {
            SetSelectedContact(stationHeadInstance);
        });

        contactVisual.Add(stationHeadInstance); // Add the station head to the contact visual container

        // For each contact (non station head), write out the contents of that contact and place it in the container
        foreach (Contact contactObject in station.contacts)
        {
            var contactInstance = contactCard.Instantiate();
            contactInstance.Q<Label>("contact-name").text = contactObject.Name;
            contactInstance.Q<Label>("contact-type").text = VulturaInstance.enumStringParser(contactObject.Type.ToString());
            contactInstance.userData = contactObject;
            contactInstance.RegisterCallback<ClickEvent>(ev => {
                SetSelectedContact(contactInstance);
            });

            contactVisual.Add(contactInstance);
        }
    }

    // Initialize the beginning of the conversation stack baseed on which contact was selected
    public void InitializeConversationStack()
    {
        if (selectedContact == null)
        {
            convoStack = null;
        }
        else
        {
            convoStack = new ConversationStack();
            convoStack.Push((selectedContact.userData as Contact).Conversation);
        }

        DisplayConvo();
    }

    // Displays the convo based on the conversation at the top
    public void DisplayConvo()
    {
        // If there is no convo stack, make it show the "no convostack" visual element
        if (convoStack == null)
        {
            contactRoot.Q<Label>("contact-empty").style.display = DisplayStyle.Flex;

            contactRoot.Q<VisualElement>("contact-text-top").style.display = DisplayStyle.None;
            contactRoot.Q<VisualElement>("contact-text-body").style.display = DisplayStyle.None;
            contactRoot.Q<VisualElement>("contact-text-footer").style.display = DisplayStyle.None;
        }
        else
        {
            // Set the current contact
            Contact currContact = selectedContact.userData as Contact;

            // Set the contact information
            contactRoot.Q<Label>("contact-text-name").text = currContact.Name;
            contactRoot.Q<Label>("contact-text-faction").text = currContact.Faction;
            contactRoot.Q<VisualElement>("contact-text-top").style.display = DisplayStyle.Flex;

            // Set the paragraph content and show the prompt
            contactRoot.Q<Label>("contact-text-paragraph").text = convoStack.Top().Prompt;
            contactRoot.Q<VisualElement>("contact-text-body").style.display = DisplayStyle.Flex;

            // For up to 3 responses, display each response
            for (int i = 0; i < 3; i++)
            {
                Conversation topConvo = convoStack.Top();

                if (i < topConvo.Responses.Count)
                {
                    BaseResponse currResponse = convoStack.Top().Responses[i];
                    contactRoot.Q<Label>("contact-choice" + (i+1)).text = (i+1).ToString() + ". " + convoStack.Top().Responses[i].Prompt;
                    contactRoot.Q<Label>("contact-choice" + (i+1)).userData = currResponse;
                    contactRoot.Q<Label>("contact-choice" + (i+1)).UnregisterCallback<ClickEvent>(ResponseClick);
                    contactRoot.Q<Label>("contact-choice" + (i+1)).RegisterCallback<ClickEvent>(ResponseClick);
                    contactRoot.Q<Label>("contact-choice" + (i+1)).style.display = DisplayStyle.Flex;
                }
                else
                {
                    contactRoot.Q<Label>("contact-choice" + (i+1)).style.display = DisplayStyle.None;
                }
                
            }

            // Show what else is needed
            contactRoot.Q<VisualElement>("contact-text-footer").style.display = DisplayStyle.Flex;

            contactRoot.Q<Label>("contact-empty").style.display = DisplayStyle.None;
        }
    }

    // Set the selected content based on which one is selected. Then initialize the beginning of the conversation
    public void SetSelectedContact(VisualElement contactInstance)
    {
        if (selectedContact == null)
        {
            contactInstance.EnableInClassList("selected", true);
            selectedContact = contactInstance;
        }
        else if (contactInstance == selectedContact)
        {
            contactInstance.EnableInClassList("selected", false);
            selectedContact = null;
        }
        else
        {
            selectedContact.EnableInClassList("selected", false);
            contactInstance.EnableInClassList("selected", true);
            selectedContact = contactInstance;
        }

        if ((selectedContact.userData as Contact).Conversation != null)
            InitializeConversationStack();
    }

    // When a conversation response is clicked, handle it
    public void ResponseClick(ClickEvent ev)
    {
        Label contactLabel = ev.currentTarget as Label;
        Debug.Log(contactLabel.userData);
        HandleResponses(contactLabel.userData as BaseResponse);
    }

    // Add the conversation to the top of the stack (based on the response selected)
    public void HandleResponses(BaseResponse response)
    {
        if (response.Type == VulturaInstance.ResponseType.Basic)
        {
            BasicResponse currResponse = response as BasicResponse;
            if (currResponse.GoBack)
            {
                convoStack.Pop();

                if (convoStack.IsEmpty())
                {
                    convoStack = null;
                    selectedContact.EnableInClassList("selected", false);
                    selectedContact = null;
                }  
            }
            else
            {
                if (currResponse.Conversation != null)
                {
                    Debug.Log("Pushing convo");
                    Debug.Log(currResponse.Prompt);
                    convoStack.Push(currResponse.Conversation);
                    convoStack.DisplayConvoStack();
                } 
            }
            
            DisplayConvo();
        }
    }

    // Go to the market page
    public void InitializeMarket()
    {
        // Turn off all game objects besides the market
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
        marketGameobject.SetActive(true);
        cargoGameobject.SetActive(false);
        facilityGameobject.SetActive(false);

        // Set the market root visual element
        marketRoot = marketGameobject.GetComponent<UIDocument>().rootVisualElement;

        // Grab all the listviews
        inventoryList = marketRoot.Q<ListView>("inventory-list");
        storageList = marketRoot.Q<ListView>("storage-list");
        marketList = marketRoot.Q<ListView>("market-list");

        // Grab the quantity slider
        quantitySlider = marketRoot.Q<SliderInt>("bottom-slider");

        // When the quantity value changes, set the price information and set the current quantity (which holds the # of items the user wants to buy)
        quantitySlider.RegisterValueChangedCallback(ev => {
            currQuantity = ev.newValue;
            marketRoot.Q<Label>("bottom-item-quantity").text = ev.newValue.ToString();
            marketRoot.Q<Label>("bottom-item-buy-price").text = "$" + (station.market.itemList[selectedIndex].buyPrice * currQuantity).ToString();
        });

        // Set the none display active initially, because nothing is selected
        marketRoot.Q<VisualElement>("none-display").style.display = DisplayStyle.Flex;
        marketRoot.Q<VisualElement>("purchase-display").style.display = DisplayStyle.None;

        // When a market item is made, instantiate a marketRow item
        Func<VisualElement> makeItemMarket = () => marketRow.Instantiate();

        // Bind values to the marketRow item when it's instantiated
        Action<VisualElement, int> bindItemMarket = (e, i) => {

            BindMarketItem(e, i);
        };

        // When an inventory item is made, instantiate an inventoryRow item
        Func<VisualElement> makeItemInventory = () => inventoryRow.Instantiate();

        // Bind values to the inventoryRow item when it's instantiated
        Action<VisualElement, int> bindItemInventory = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = playerInventory.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = playerInventory.itemList[i].quantity.ToString();

            e.RegisterCallback<ClickEvent>(ev => {
                // TODO -- Create selling when item in inventory is clicked
            });
        };

        // Bind values to the inventoryRow item when it's instantiated
        Action<VisualElement, int> bindItemStorage = (e, i) => {
            var itemName = e.Q<Label>("item-name");
            itemName.text = station.storage.itemList[i].item.Name;

            var itemQuantity = e.Q<Label>("item-quantity");
            itemQuantity.text = station.storage.itemList[i].quantity.ToString();

            e.RegisterCallback<ClickEvent>(ev => {
                // TODO -- Create selling when item in storage is clicked
            });
        };

        // Bind all the values to the marketList
        marketList.makeItem = makeItemMarket;
        marketList.bindItem = bindItemMarket;
        marketList.itemsSource = station.market.itemList;

        // Bind all the values to the inventoryList
        inventoryList.makeItem = makeItemInventory;
        inventoryList.bindItem = bindItemInventory;
        inventoryList.itemsSource = playerInventory.itemList;

        // Bind all the values to the storageList
        storageList.makeItem = makeItemInventory;
        storageList.bindItem = bindItemStorage;
        storageList.itemsSource = station.storage.itemList;

        // Set the back button to go back to the home
        marketRoot.Q<Button>("back-button").RegisterCallback<ClickEvent>(ev => {
            InitializeHome();
        });
    }

    // Bind thhe market item values
    public void BindMarketItem(VisualElement e, int i)
    {
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

        // When a market item is pressed, set the current selected item and set all the bottom item values to that item's values
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

            // If the market item is not sell only, let them buy the item
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
                // If buy is clicked on, buy the item
                buyButton.RegisterCallback<ClickEvent>(ev => {
                    int itemId = station.market.itemList[i].item.Id;
                    BuyItem();

                    quantitySlider.highValue = station.market.itemList[i].quantity;

                    if (itemId != station.market.itemList[i].item.Id)
                    {
                        marketRoot.Q<VisualElement>("none-display").style.display = DisplayStyle.Flex;
                        marketRoot.Q<VisualElement>("purchase-display").style.display = DisplayStyle.None;
                    }
                });
            }
            else
            {
                // Instead, set the text to "sell" and only let them sell the item
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

                // If button is clicked, sell the item and set the quantity slider to the new quantity
                buyButton.RegisterCallback<ClickEvent>(ev => {
                    SellItem();

                    quantitySlider.highValue = itemInInventory.quantity;
                });
            }
                
  
        });
    }

    // Sell the item to the station's stockpile
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

    // Buy the item fromo the station and put it in your inventory
    void BuyItem()
    {
        int totalCost = station.market.itemList[selectedIndex].buyPrice * currQuantity;

        if (VulturaInstance.playerMoney >= totalCost)
        {
            InventoryItem purchasedItem = station.market.Purchase(selectedIndex, currQuantity);
            
            if (purchasedItem != null)
            {
                bool successfulAdd = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.AddToCargo(purchasedItem);

                if (!successfulAdd)
                    station.storage.Add(purchasedItem);
                inventoryList.Rebuild();
                marketList.Rebuild();

                VulturaInstance.playerMoney -= totalCost;

            }
        }
    }

    // Initialize the cargo page
    void InitializeCargo()
    {
        marketGameobject.SetActive(false);
        cargoGameobject.SetActive(true);
        contactGameobject.SetActive(false);
        homeGameobject.SetActive(false);
        facilityGameobject.SetActive(false);

        cargoRoot = cargoGameobject.GetComponent<UIDocument>().rootVisualElement;

        cargoRoot.Q<Button>("button-back").RegisterCallback<ClickEvent>(ev => {
            InitializeHome();
        });

        DisplayContract();

        // Set the cargo list container  visual element
        VisualElement cargoList = cargoRoot.Q<VisualElement>("contract-list");

        // For every conotract in the station, create a visual element instance, set its values, and place it
        foreach (Contract contract in station.contracts)
        {
            VisualElement cargoInstance = contractItem.Instantiate();
            string title = "";

            foreach (InventoryItem item in contract.Items.itemList)
            {
                if (title == "")
                {
                    title += item.quantity.ToString() + " " + item.item.Name;
                }
                else
                {
                    title += ", " + item.quantity.ToString() + " " + item.item.Name;
                }
            }

            cargoInstance.Q<Label>("contract-title").text = title;
            cargoInstance.Q<Label>("contract-distance").text = UnityEngine.Random.Range(2, 17).ToString() + " jumps away";
            cargoInstance.userData = contract;

            // Select a contract by clicking on it
            cargoInstance.RegisterCallback<ClickEvent>(ev => {
                if (selectedContract == null)
                {
                    cargoInstance.EnableInClassList("selected", true);
                    selectedContract = cargoInstance;
                }
                else if (selectedContract == cargoInstance)
                {
                    selectedContract.EnableInClassList("selected", false);
                    selectedContract = null;
                }
                else
                {
                    selectedContract.EnableInClassList("selected", false);
                    cargoInstance.EnableInClassList("selected", true);
                    selectedContract = cargoInstance;
                }

                DisplayContract();
            });
            
            cargoList.Add(cargoInstance);
        }
    }

    // Display a cargo contract to the screen
    void DisplayContract()
    {
        // If there is no selected contract, display empty visual element
        if (selectedContract == null)
        {
            cargoRoot.Q<Label>("contract-empty").style.display = DisplayStyle.Flex;
            cargoRoot.Q<VisualElement>("contract-selected").style.display = DisplayStyle.None;
        }
        else
        {
            cargoRoot.Q<Label>("contract-empty").style.display = DisplayStyle.None;

            VisualElement itemList = cargoRoot.Q<VisualElement>("contract-items");
            itemList.Clear();

            // Display items for each item in the contract
            foreach (InventoryItem item in (selectedContract.userData as Contract).Items.itemList)
            {
                VisualElement itemInstance = currentContractItem.Instantiate();
                itemInstance.Q<Label>("item-name").text = item.quantity.ToString() + " " + item.item.Name;
                itemList.Add(itemInstance);
            }

            cargoRoot.Q<Label>("contract-destination").text = "Destination: " + (selectedContract.userData as Contract).Destination;

            cargoRoot.Q<VisualElement>("contract-selected").style.display = DisplayStyle.Flex;
        }
    }
}
