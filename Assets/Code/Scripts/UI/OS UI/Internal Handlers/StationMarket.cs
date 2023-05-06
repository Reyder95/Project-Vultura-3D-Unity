using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class StationMarket : OSUIHandler
{
    public UnityAction marketListener;

    // Market page information
    VisualElement marketVisualElement;      // current page
    
    // Various visual element references
    VisualElement itemHeaderContent;
    VisualElement itemHeaderNoContent;
    VisualElement itemDescription;
    VisualElement buySellSection;
    VisualElement pricingContainer;
    VisualElement purchaseSection;
    VisualElement transactionButton;
    VisualElement buyItemPrice;
    VisualElement sellItemPrice;
    ListView marketList;                    // The non-demand market listview (for buying)
    ListView demandList;                    // The demand market listview  (for selling)
    VisualElement currentMarketSelection;   // The current element selected (from the lists)
    int currentInventorySellSelection = -1; // The current inventory selection if trying to sell from inventory
    int demandMarketIndex = -1;             // If selling from inventory and element is in demand, this is the index used
    Market marketElement;                   // The market element we are using (demand or non-demand)
    int quantSliderValue = -1;              // The quantity that we want to buy or sell
    int itemPrice = -1;                     // The item price of the item selected
    SellMode sellMode = SellMode.BUY;       // The sell mode. If we are buying or selling (used in transaction button event)
    int marketHovered = -1;                 // which element is hovered to color the lists
    bool isMarketHovered = false;           // Checks if we are hovering over demand or market
    SliderInt quantSlider;
    Label quantityAmount;
    Label purchaseCurrency;
    bool marketSwitch = false;

    public override void SetTaggedReferences(VisualElement screen, StationUI station)
    {
        marketListener = new UnityAction(RefreshMarket);

        uiComponent = station;

        marketVisualElement = screen.Q<VisualElement>("station-market");

        itemHeaderContent = marketVisualElement.Q<VisualElement>("item-header-content");
        itemHeaderNoContent = marketVisualElement.Q<VisualElement>("item-header-no-content");
        itemDescription = marketVisualElement.Q<VisualElement>("item-description");
        buySellSection = marketVisualElement.Q<VisualElement>("buy-sell-section");
        pricingContainer = marketVisualElement.Q<VisualElement>("pricing-container");
        purchaseSection = marketVisualElement.Q<VisualElement>("purchase-section");
        transactionButton = marketVisualElement.Q<VisualElement>("transaction-button");
        marketList = marketVisualElement.Q<ListView>("sale-market");
        demandList = marketVisualElement.Q<ListView>("sale-demand");
        buyItemPrice = marketVisualElement.Q<VisualElement>("buy-item-price");
        sellItemPrice = marketVisualElement.Q<VisualElement>("sell-item-price");
        quantSlider = marketVisualElement.Q<SliderInt>("quantity-slider");
        quantityAmount = marketVisualElement.Q<Label>("quantity-amount");
        purchaseCurrency = marketVisualElement.Q<Label>("purchase-currency");

        marketVisualElement.style.opacity = 0.0f;
        itemHeaderContent.style.opacity = 0.0f;
        itemHeaderNoContent.style.opacity = 1.0f;
        itemDescription.style.opacity = 0.0f;
        buySellSection.style.opacity = 0.0f;
        pricingContainer.style.opacity = 0.0f;
        purchaseSection.style.opacity = 0.0f;

        marketVisualElement.style.display = DisplayStyle.None;
        itemHeaderContent.style.display = DisplayStyle.None;
        itemHeaderNoContent.style.display = DisplayStyle.Flex;
        itemDescription.style.display = DisplayStyle.None;
        buySellSection.style.display = DisplayStyle.None;
        pricingContainer.style.display = DisplayStyle.None;
        purchaseSection.style.display = DisplayStyle.None;
    }

    public override void SetCallbacks()
    {
        marketVisualElement.RegisterCallback<TransitionEndEvent>(MarketPageEndTransition);
        marketVisualElement.RegisterCallback<PointerUpEvent>(MarketPagePointerUp);
        itemHeaderContent.RegisterCallback<TransitionEndEvent>(ItemHeaderContentEndTransition);
        transactionButton.RegisterCallback<ClickEvent>(TransactionEvent);
        itemHeaderNoContent.RegisterCallback<TransitionEndEvent>(ItemHeaderNoContentEndTransition);
    }

    public override VisualElement ReturnPage()
    {
        return marketVisualElement;
    }

    public void ClearData()
    {
        currentMarketSelection = null;
        marketList = null;
        demandList = null;
    }

    // TODO condense both buy and both sell functions to singular functions rather than using 4 (2 instead of 4)
    void BuyItemCallback(ClickEvent ev)
    {
        BuyItem(marketElement, (currentMarketSelection.userData as MarketSelectionData).elementIndex, quantSliderValue);
    }

    void SellItemDemandCallback(ClickEvent ev)
    {
        SellItemDemand((currentMarketSelection.userData as MarketSelectionData).elementIndex, quantSliderValue);
    }

    // Load up the market into the list views
    public void LoadMarket()
    {        

        Func<VisualElement> makeItemMarket = () => uiComponent.loadableAssets["market-item"].Instantiate();

        Action<VisualElement, int> bindItemMarket = (e, i) => {
            e.Q<Label>("item-name").text = uiComponent.currentStation.market.itemList[i].item.Name + " / " + uiComponent.currentStation.market.itemList[i].quantity.ToString() + "x";
            e.Q<Label>("item-price").text = "Buy: $" + uiComponent.currentStation.market.itemList[i].buyPrice.ToString();

            if (currentMarketSelection != null && (currentMarketSelection.userData as MarketSelectionData).elementIndex == i && (currentMarketSelection.userData as MarketSelectionData).isMarket)
            {
                e.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                currentMarketSelection = e;
            }   
            else
                e.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));

            if (currentMarketSelection != e)
            {
                if (i == marketHovered && isMarketHovered == true)
                {
                    e.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                }
                else if (i != marketHovered && isMarketHovered == true)
                {
                    e.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
                }
            }

            MarketSelectionData newData = new MarketSelectionData(i, true, uiComponent.currentStation.market.itemList[i].item);
            e.userData = newData;

            e.RegisterCallback<ClickEvent>(ev => {

                if (currentInventorySellSelection != -1)
                    marketSwitch = true;

                currentInventorySellSelection = -1;

                if ((ev.currentTarget as VisualElement) != currentMarketSelection)
                    SetCurrentMarketItem(ev.currentTarget as VisualElement);
                else
                    SetCurrentMarketItem(null);
            });

            e.RegisterCallback<PointerEnterEvent>(ev => {

                if (currentMarketSelection != null)
                {
                    int selectedElement = (currentMarketSelection.userData as MarketSelectionData).elementIndex;

                    if ((currentMarketSelection.userData as MarketSelectionData).isMarket)
                    {
                        if (i != selectedElement)
                        {
                            VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                            element.style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                        }
                    }
                    else
                    {
                        VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                        element.style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                    }
                }
                else
                {
                    VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                    element.style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                }

                marketHovered = i;
                isMarketHovered = true;
            });

            e.RegisterCallback<PointerLeaveEvent>(ev => {
                if (currentMarketSelection != null)
                {
                    int selectedElement = (currentMarketSelection.userData as MarketSelectionData).elementIndex;

                    if ((currentMarketSelection.userData as MarketSelectionData).isMarket)
                    {
                        if (i != selectedElement)
                        {
                            VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                            element.style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
                        }
                    }
                    else
                    {
                        VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                        element.style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
                    }
                }
                else
                {
                        VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                        element.style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
                }

                marketHovered = -1;
            });


        };

        marketList.makeItem = makeItemMarket;
        marketList.bindItem = bindItemMarket;
        marketList.itemsSource = uiComponent.currentStation.market.itemList;

        Func<VisualElement> makeItemDemand = () => uiComponent.loadableAssets["market-item"].Instantiate();

        Action<VisualElement, int> bindItemDemand = (e, i) => {
            e.Q<Label>("item-name").text = uiComponent.currentStation.demandMarket.itemList[i].item.Name;
            e.Q<Label>("item-price").text = "Sell: $" + uiComponent.currentStation.demandMarket.itemList[i].sellPrice.ToString();

            MarketSelectionData newData = new MarketSelectionData(i, false, uiComponent.currentStation.demandMarket.itemList[i].item);
            e.userData = newData;

            if (demandMarketIndex != -1)
            {
                SetCurrentMarketItem(e);
                Debug.Log("Test!");
                demandMarketIndex = -1;
            }

            if (currentMarketSelection != null && (currentMarketSelection.userData as MarketSelectionData).elementIndex == i && !(currentMarketSelection.userData as MarketSelectionData).isMarket)
            {
                e.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                currentMarketSelection = e;
            }   
            else
                e.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));

            if (currentMarketSelection != e)
            {
                if (i == marketHovered && isMarketHovered == false)
                {
                    e.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                }
                else if (i == marketHovered && isMarketHovered == false)
                {
                    e.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
                }
            }

            e.RegisterCallback<ClickEvent>(ev => {

                if (currentInventorySellSelection != -1)
                    marketSwitch = true;

                currentInventorySellSelection = -1;

                if ((ev.currentTarget as VisualElement) != currentMarketSelection)
                    SetCurrentMarketItem(ev.currentTarget as VisualElement);
                else
                    SetCurrentMarketItem(null);
            });

            e.RegisterCallback<PointerEnterEvent>(ev => {

                if (currentMarketSelection != null)
                {
                    int selectedElement = (currentMarketSelection.userData as MarketSelectionData).elementIndex;

                    if (!(currentMarketSelection.userData as MarketSelectionData).isMarket)
                    {
                        if (i != selectedElement)
                        {
                            VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                            element.style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                        }
                    }
                    else
                    {
                        VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                        element.style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                    }
                }
                else
                {
                    VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                    element.style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
                }

                marketHovered = i;
                isMarketHovered = false;
            });

            e.RegisterCallback<PointerLeaveEvent>(ev => {
                if (currentMarketSelection != null)
                {
                    int selectedElement = (currentMarketSelection.userData as MarketSelectionData).elementIndex;

                    if (!(currentMarketSelection.userData as MarketSelectionData).isMarket)
                    {
                        if (i != selectedElement)
                        {
                            VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                            element.style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
                        }
                    }
                    else
                    {
                        VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                        element.style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
                    }
                }
                else
                {
                        VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
                        element.style.backgroundColor = new StyleColor(new Color32(104, 124, 227, 51));
                }

                marketHovered = -1;
            });
        };

        demandList.makeItem = makeItemDemand;
        demandList.bindItem = bindItemDemand;
        demandList.itemsSource = uiComponent.currentStation.demandMarket.itemList;
    }

    // Handle the sell mode each frame, which changes things accordingly
    public void HandleSellMode()
    {
        if (currentInventorySellSelection != -1)
        {
            sellMode = SellMode.SELL;
        }
        else
        {
            if (currentMarketSelection != null)
            {
                if ((currentMarketSelection.userData as MarketSelectionData).isMarket)
                {
                    sellMode = SellMode.BUY;
                }
                else
                {
                    sellMode = SellMode.SELL;
                }
            }
        }

        if (currentInventorySellSelection != -1 && currentMarketSelection != null)
            SetCurrentMarketItem(null);
    }

    // When the market page ends a transition of opacity
    private void MarketPageEndTransition(TransitionEndEvent ev)
    {
        if (ev.stylePropertyNames.Contains("opacity"))
        {
            VisualElement element = (ev.currentTarget as VisualElement);
            if (element.style.opacity == 0.0f)
            {
                Debug.Log("Test!");
                if (!uiComponent.back)
                    uiComponent.HandlePageTransition(uiComponent.DeterminePageElement());
                else
                {
                    uiComponent.pageStack.Pop();
                    uiComponent.pageStack.Top().style.opacity = 1.0f;
                    uiComponent.back = false;
                }
            }
        }
    }

    // When the market page is hovered and the user lets go of left click
    private void MarketPagePointerUp(PointerUpEvent ev)
    {
        if (MasterOSManager.Instance.isDragging)
        {

            Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
            currentInventorySellSelection = (int)MasterOSManager.Instance.currentDraggedElement.userData;   
            ExistsStruct existsValue = uiComponent.currentStation.demandMarket.ContainsItem(playerInventory.itemList[currentInventorySellSelection].item); 

            
            itemHeaderContent.style.opacity = 0.0f;
            itemDescription.style.opacity = 0.0f;
            buySellSection.style.opacity = 0.0f;
            pricingContainer.style.opacity = 0.0f;
            purchaseSection.style.opacity = 0.0f;

            if (currentMarketSelection != null)
                marketSwitch = true;

            if (!existsValue.exists)
            {
                SetCurrentMarketItem(null, true);
            }
            else
            {
                demandMarketIndex = existsValue.index; 

                if (itemHeaderNoContent.style.opacity != 0.0f)
                    itemHeaderNoContent.style.opacity = 0.0f;

                if (currentMarketSelection != null || currentInventorySellSelection != -1)
                {
                    currentMarketSelection = null;   
                }
                else if (currentMarketSelection != null)
                {
                    currentInventorySellSelection = -1;
                    RefreshMarket();
                }
            }

            marketList.Rebuild();
            demandList.Rebuild();
        }
    }

    // Whenn the item header content ends a transition of opacity
    private void ItemHeaderContentEndTransition(TransitionEndEvent ev)
    {
        if (ev.stylePropertyNames.Contains("opacity"))
        {
            VisualElement element = (ev.currentTarget as VisualElement);
            if (element.style.opacity == 0.0f)
            {
                if (currentMarketSelection != null || currentInventorySellSelection != -1)
                {
                    DisplayMarket();
                    itemHeaderContent.style.opacity = 1.0f;
                    itemDescription.style.opacity = 1.0f;
                    buySellSection.style.opacity = 1.0f;
                    pricingContainer.style.opacity = 1.0f;
                    purchaseSection.style.opacity = 1.0f;
                }
                else
                {
                    itemHeaderContent.style.display = DisplayStyle.None;
                    itemDescription.style.display = DisplayStyle.None;
                    buySellSection.style.display = DisplayStyle.None;
                    pricingContainer.style.display = DisplayStyle.None;
                    purchaseSection.style.display = DisplayStyle.None;
                    itemHeaderNoContent.style.display = DisplayStyle.Flex;
                    
                    itemHeaderNoContent.style.opacity = 1.0f;
                }
            }
        }
    }

    // When the transaction button is pressed, a buy call or sell call will happen
    private void TransactionEvent(ClickEvent ev)
    {
        if (sellMode == SellMode.BUY)
        {
            BuyItem(marketElement, (currentMarketSelection.userData as MarketSelectionData).elementIndex, quantSliderValue);
        }
        else if (sellMode == SellMode.SELL)
        {
            if (currentInventorySellSelection != -1)
            {
                itemPrice = quantSliderValue * (int)Math.Floor(VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.itemList[currentInventorySellSelection].item.GalacticPrice * 0.75f * 0.50f);
                    SellItemInventory(itemPrice);
                    DisplayMarket();
            }
            else
            {
                SellItemDemand((currentMarketSelection.userData as MarketSelectionData).elementIndex, quantSliderValue);
            }
        }
    }

    private void ItemHeaderNoContentEndTransition(TransitionEndEvent ev)
    {
        if (ev.stylePropertyNames.Contains("opacity"))
        {
            VisualElement element = (ev.currentTarget as VisualElement);
            if (element.style.opacity == 0.0f)
            {
                if (currentInventorySellSelection != -1)
                    DisplayMarket();
                    
                itemHeaderNoContent.style.display = DisplayStyle.None;
                itemDescription.style.display = DisplayStyle.Flex;
                itemHeaderContent.style.display = DisplayStyle.Flex;
                buySellSection.style.display = DisplayStyle.Flex;
                pricingContainer.style.display = DisplayStyle.Flex;
                purchaseSection.style.display = DisplayStyle.Flex;
                itemHeaderContent.style.opacity = 1.0f;
                itemDescription.style.opacity = 1.0f;
                buySellSection.style.opacity = 1.0f;
                pricingContainer.style.opacity = 1.0f;
                purchaseSection.style.opacity = 1.0f;
            }
        }
    }

    private void SetCurrentMarketItem(VisualElement marketItem, bool forceSwitch = false)
    {
        if (forceSwitch)
            marketSwitch = true;

        if (marketItem != null)
        {
            if (currentMarketSelection != null || marketSwitch)
            {
                marketSwitch = false;
                
                if (currentMarketSelection != null)
                    currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(104, 124,227, 51));

                itemHeaderContent.style.opacity = 0.0f;
                itemDescription.style.opacity = 0.0f;
                buySellSection.style.opacity = 0.0f;
                pricingContainer.style.opacity = 0.0f;
                purchaseSection.style.opacity = 0.0f;

                currentMarketSelection = marketItem;

                currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
            }
            else
            {
                if (currentInventorySellSelection == -1)
                {
                    currentMarketSelection = marketItem;
    
                    currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));

                    DisplayMarket();
                }
                else
                {
                    currentMarketSelection = marketItem;

                    currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));

                    DisplayMarket();
                }
            }
        }
        else 
        {
            currentMarketSelection = null;

            if (marketSwitch)
            {
                marketSwitch = false;
                
                if (itemHeaderNoContent.style.opacity != 0.0f)
                    itemHeaderNoContent.style.opacity = 0.0f;
                else
                {
                    itemHeaderContent.style.opacity = 0.0f;
                    itemDescription.style.opacity = 0.0f;
                    buySellSection.style.opacity = 0.0f;
                    pricingContainer.style.opacity = 0.0f;
                    purchaseSection.style.opacity = 0.0f;
                }
            }
            else
            {
                quantSliderValue = -1;

                DisplayMarket();
            }
        }
    }

    public void RefreshMarket()
    {
        try
        {       
            marketList.Rebuild();
            demandList.Rebuild();
            
            if (currentMarketSelection != null)
            {
                int itemIndex = (currentMarketSelection.userData as MarketSelectionData).elementIndex;
                bool isMarket = (currentMarketSelection.userData as MarketSelectionData).isMarket;
                BaseItem itemReference = (currentMarketSelection.userData as MarketSelectionData).itemReference;

                if (isMarket)
                    marketElement = uiComponent.currentStation.market;
                else
                    marketElement = uiComponent.currentStation.demandMarket;

                if (marketElement.itemList.Count > 0)
                {
                    if (marketElement.itemList[itemIndex].item.Key != itemReference.Key)
                    {
                        currentMarketSelection = null;
                        quantSliderValue = -1;
                    }
                }
                else
                {
                    currentMarketSelection = null;
                    quantSliderValue = -1;
                }
            }

            DisplayMarket();
        } catch (System.NullReferenceException ex)
        {
            Debug.Log("Refresh market returned a null value");
        }
        catch (System.ArgumentOutOfRangeException ex)
        {
            Debug.Log("An index is out of range!");
        }
    }

    private void DisplayMarket()
    {
        Debug.Log(currentInventorySellSelection);

        if (currentInventorySellSelection == -1)
        {
            if (currentMarketSelection != null)
            {
                MarketSelectionData marketSelectionData = currentMarketSelection.userData as MarketSelectionData;

                if (marketSelectionData.isMarket)
                    marketElement = uiComponent.currentStation.market;
                else
                    marketElement = uiComponent.currentStation.demandMarket;

                itemHeaderContent.Q<Label>("item-name").text = marketElement.itemList[marketSelectionData.elementIndex].item.Name;
                (itemDescription as Label).text = marketElement.itemList[marketSelectionData.elementIndex].item.Description;
                (buyItemPrice as Label).text = "Buy 1 for $" + marketElement.itemList[marketSelectionData.elementIndex].buyPrice.ToString();
                (sellItemPrice as Label).text = "Sell 1 for $" + marketElement.itemList[marketSelectionData.elementIndex].sellPrice.ToString();

                if (marketSelectionData.isMarket)
                {
                    transactionButton.Q<Label>("transaction-text").text = "Buy";

                    transactionButton.EnableInClassList("disabled", false);
                    transactionButton.EnableInClassList("main-button", true);

                    quantSlider.lowValue = 1;
                    quantSlider.highValue = marketElement.itemList[marketSelectionData.elementIndex].quantity;

                    if (quantSliderValue < 0)
                        quantSlider.value = 1;
                    else
                        quantSlider.value = quantSliderValue;

                    quantSlider.RegisterValueChangedCallback(ev => {
                        try
                        {
                            quantSliderValue = ev.newValue;
                            quantityAmount.text = ev.newValue.ToString();
                            purchaseCurrency.text = "$" + (marketElement.itemList[marketSelectionData.elementIndex].buyPrice * ev.newValue).ToString();
                        } catch(System.ArgumentOutOfRangeException ex)
                        {
                            Debug.Log("An argument was out of range.");
                        }

                    });
                }
                else
                {
                    Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

                    BaseItem selectedItem = marketElement.itemList[marketSelectionData.elementIndex].item;

                    InventoryItem playerInvItem = playerInventory.FindItem(selectedItem);

                    transactionButton.Q<Label>("transaction-text").text = "Sell";

                    if (playerInvItem != null)
                    {
                        quantSlider.lowValue = 1;
                        quantSlider.highValue = playerInvItem.quantity;

                        if (quantSliderValue < 0)
                            quantSlider.value = 1;
                        else
                            quantSlider.value = quantSliderValue;

                        quantSlider.RegisterValueChangedCallback(ev => {
                            HandleQuantSlider(ev, marketElement, marketSelectionData.elementIndex);
                        });

                        transactionButton.EnableInClassList("disabled", false);
                        transactionButton.EnableInClassList("main-button", true);
                    }
                    else
                    {
                        quantSlider.lowValue = 0;
                        quantSlider.highValue = 0;
                        quantSlider.value = 0;

                        quantSlider.UnregisterValueChangedCallback(ev => {
                            HandleQuantSlider(ev, marketElement, marketSelectionData.elementIndex);
                        });

                        quantityAmount.text = "0";
                        purchaseCurrency.text = "N/A";

                        transactionButton.EnableInClassList("disabled", true);
                        transactionButton.EnableInClassList("main-button", false);
                    }

                }

                itemHeaderNoContent.style.opacity = 0.0f;

            } 
            else 
            {
                itemHeaderContent.style.opacity = 0.0f;
                itemDescription.style.opacity = 0.0f;
                buySellSection.style.opacity = 0.0f;
                pricingContainer.style.opacity = 0.0f;
                purchaseSection.style.opacity = 0.0f;
            }
        } 
        else
        {
            try
            {
                Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
                int itemPrice = (int)Math.Floor(playerInventory.itemList[currentInventorySellSelection].item.GalacticPrice * 0.75f * 0.50f);


                itemHeaderContent.Q<Label>("item-name").text = playerInventory.itemList[currentInventorySellSelection].item.Name;
                (itemDescription as Label).text = playerInventory.itemList[currentInventorySellSelection].item.Description;
                (buyItemPrice as Label).text = "Buy 1 for $0";
                (sellItemPrice as Label).text = "Sell 1 for $" + itemPrice.ToString();

                transactionButton.Q<Label>("transaction-text").text = "Sell";

                quantSlider.lowValue = 1;
                quantSlider.highValue = playerInventory.itemList[currentInventorySellSelection].quantity;

                if (quantSliderValue < 0)
                    quantSlider.value = 1;
                else
                    quantSlider.value = quantSliderValue;

                quantSlider.RegisterValueChangedCallback(ev => {
                    HandleQuantSliderInv(ev, itemPrice);
                });
                
                transactionButton.EnableInClassList("disabled", false);
                transactionButton.EnableInClassList("main-button", true);

                // marketVisualElement.Q<VisualElement>("transaction-button").UnregisterCallback<ClickEvent>(SellItemInventoryEvent);

                // marketVisualElement.Q<VisualElement>("transaction-button").RegisterCallback<ClickEvent>(SellItemInventoryEvent);
            } catch (System.ArgumentOutOfRangeException ex)
            {
                Debug.Log("Argument out of range!");

                itemHeaderContent.style.opacity = 0.0f;
                itemDescription.style.opacity = 0.0f;
                buySellSection.style.opacity = 0.0f;
                pricingContainer.style.opacity = 0.0f;
                purchaseSection.style.opacity = 0.0f;
            }

            itemHeaderNoContent.style.opacity = 0.0f;
        }  
    }

    private void SellItemInventoryEvent(ClickEvent ev)
    {
        SellItemInventory(itemPrice);
        DisplayMarket();
    }

    private void SellItemDemandEvent(ClickEvent ev)
    {
        SellItemDemand((currentMarketSelection.userData as MarketSelectionData).elementIndex, quantSliderValue);
    }

    private void HandleQuantSlider(ChangeEvent<int> ev, Market marketElement, int elementIndex)
    {
        quantityAmount.text = ev.newValue.ToString();
        purchaseCurrency.text = "$" + (marketElement.itemList[elementIndex].sellPrice * ev.newValue).ToString();
    }

    private void HandleQuantSliderInv(ChangeEvent<int> ev, int itemPrice)
    {
        quantSliderValue = ev.newValue;
        quantityAmount.text = ev.newValue.ToString();
        purchaseCurrency.text = "$" + (itemPrice * ev.newValue).ToString();
    }

    private bool BuyItem(Market market, int index, int quantity)
    {
        int totalCost = market.itemList[index].buyPrice * quantity;
        Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
        float totalWeight = market.itemList[index].item.Weight * quantity;

        if (playerInventory.CargoFullWithAmount(totalWeight, VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip))
        {
            return false;
        }

        if (totalCost > VulturaInstance.playerMoney)
        {
            return false;
        }

        InventoryItem purchasedItem = market.Purchase(index, quantity);

        if (purchasedItem != null)
        {
            VulturaInstance.playerMoney = VulturaInstance.playerMoney - totalCost;
            playerInventory.Add(purchasedItem, VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip);

            EventManager.TriggerEvent("Market Changed");

            return true;
        }

        return false;
    }

    private bool SellItemDemand(int index, int quantity)
    {
        Market demandMarket = uiComponent.currentStation.demandMarket;

        if (index < demandMarket.itemList.Count)
        {
            Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

            BaseItem soldItem = demandMarket.itemList[index].item;

            int inventoryItemIndex = playerInventory.FindItemIndex(soldItem);

            InventoryItem inventoryItem = playerInventory.PopAmount(inventoryItemIndex, quantity);

            if (inventoryItem != null)
            {
                int totalMoney = demandMarket.itemList[index].sellPrice * quantity;

                VulturaInstance.playerMoney += totalMoney;

                uiComponent.currentStation.InsertIntoStockpile(soldItem, quantity);

                return true;
            }
        }

        return false;
    }

    private bool SellItemInventory(int itemCost)
    {
        Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

        if (currentInventorySellSelection != -1)
        {
            InventoryItem item = playerInventory.ReturnAmountOfItem(currentInventorySellSelection, quantSliderValue);

            if (item != null)
            {
                int sellCost = itemCost * item.quantity;

                uiComponent.currentStation.market.Add(item.item, item.quantity);

                playerInventory.PopAmount(currentInventorySellSelection, item.quantity);

                VulturaInstance.playerMoney += sellCost;

                try
                {
                    if (playerInventory.itemList[currentInventorySellSelection].item.Key != item.item.Key)
                    {
                        currentInventorySellSelection = -1;

                        DisplayMarket();
                    }
                } catch (System.NullReferenceException ex)
                {
                    Debug.Log("There's a null reference exception.");
                } catch (System.ArgumentOutOfRangeException ex)
                {
                    Debug.Log("There's an argument out of range.");
                }


                return true;
            }

        }

        return false;
    }

}