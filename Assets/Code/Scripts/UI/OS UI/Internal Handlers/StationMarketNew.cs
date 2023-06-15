using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class StationMarketNew : OSUIHandler
{
    public UnityAction marketListener;

    VisualElement marketVisualElement;

    TransactionItem currItem = null;

    VisualElement itemHeaderContent;
    VisualElement itemHeaderNoContent;
    VisualElement itemDescription;
    VisualElement buySellSection;
    VisualElement pricingContainer;
    VisualElement purchaseSection;
    VisualElement transactionButton;
    VisualElement buyItemPrice;
    VisualElement sellItemPrice;
    ListView marketList;
    ListView demandList;
    VisualElement currentMarketSelection;
    SliderInt quantSlider;
    Label quantityAmount;
    Label purchaseCurrency;

    int quantSliderValue = 1;

    // Colors
    Color32 marketActiveColor = new Color32(176, 185, 232, 51);
    Color32 marketInactiveColor = new Color32(104, 124, 227, 51);

    // Handle the tagged references within the UXML file, and connect them to C# variables for ease-of-access
    public override void SetTaggedReferences(VisualElement screen, StationUI station)
    {
        marketListener = new UnityAction(RefreshMarket);

        uiComponent = station;

        marketVisualElement = screen.Q<VisualElement>("station-market");
        marketVisualElement.style.display = DisplayStyle.None;
        
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

    // Return the current page's visual element
    public override VisualElement ReturnPage()
    {
        return marketVisualElement;
    }

    // Set the callbacks of the entire page.
    public override void SetCallbacks()
    {
        marketVisualElement.RegisterCallback<TransitionEndEvent>(MarketPageEndTransition);
        marketVisualElement.RegisterCallback<PointerUpEvent>(MarketPagePointerUp);

        itemHeaderNoContent.RegisterCallback<TransitionEndEvent>(ItemHeaderNoContentEndTransition);
        itemHeaderContent.RegisterCallback<TransitionEndEvent>(ItemHeaderContentEndTransition);
    }


    // Refresh the market items and values
    public void RefreshMarket()
    {
        marketList.Rebuild();
        demandList.Rebuild();

        if (currItem != null)
        {
            HandleMarketButton(currItem);
            HandleMarketSlider(currItem);
        }
    }

    // What is this?
    public void HandleSellMode()
    {
        Debug.Log("Not Implemented!");
    }

    // Load the market items into their list views
    public void LoadMarket()
    {
        Func<VisualElement> makeItemMarket = () => uiComponent.loadableAssets["market-item"].Instantiate();

        Action<VisualElement, int> bindItemMarket = (e, i) => {
            e.Q<Label>("item-name").text = uiComponent.currentStation.market.itemList[i].item.Name + " / " + uiComponent.currentStation.market.itemList[i].quantity.ToString() + "x";
            e.Q<Label>("item-price").text = "Buy: $" + uiComponent.currentStation.market.itemList[i].buyPrice.ToString();

            TransactionItem item = new TransactionItem(VulturaInstance.TransactionLocation.SUPPLY, i, true, uiComponent.currentStation.market.itemList[i]);
            e.userData = item;

            e.RegisterCallback<PointerEnterEvent>(MarketSupplyItemHover);
            e.RegisterCallback<PointerLeaveEvent>(MarketSupplyItemUnhover);
            e.RegisterCallback<ClickEvent>(MarketSelectEvent);
        };

        Func<VisualElement> makeItemDemand = () => uiComponent.loadableAssets["market-item"].Instantiate();

        Action<VisualElement, int> bindItemDemand = (e, i) => {
            e.Q<Label>("item-name").text = uiComponent.currentStation.demandMarket.itemList[i].item.Name;
            e.Q<Label>("item-price").text = "Sell: $" + uiComponent.currentStation.demandMarket.itemList[i].sellPrice;

            TransactionItem item = new TransactionItem(VulturaInstance.TransactionLocation.DEMAND, i, false, uiComponent.currentStation.demandMarket.itemList[i]);
            e.userData = item;

            e.RegisterCallback<PointerEnterEvent>(MarketSupplyItemHover);
            e.RegisterCallback<PointerLeaveEvent>(MarketSupplyItemUnhover);
            e.RegisterCallback<ClickEvent>(MarketSelectEvent);
        };

        marketList.makeItem = makeItemMarket;
        marketList.bindItem = bindItemMarket;
        marketList.itemsSource = uiComponent.currentStation.market.itemList;

        demandList.makeItem = makeItemDemand;
        demandList.bindItem = bindItemDemand;
        demandList.itemsSource = uiComponent.currentStation.demandMarket.itemList;
        
    }

    // Choose how to display the market button based on a provided item
    public void HandleMarketButton(TransactionItem item)
    {
        transactionButton.RegisterCallback<ClickEvent>(MarketBuyEvent);
        transactionButton.UnregisterCallback<ClickEvent>(MarketSellEvent);

        if (item.Purchase)
        {
            transactionButton.Q<Label>("transaction-text").text = "Buy";

            transactionButton.EnableInClassList("disabled", false);
            transactionButton.EnableInClassList("main-button", true);

            transactionButton.RegisterCallback<ClickEvent>(MarketBuyEvent);
        }
        else
        {
            transactionButton.Q<Label>("transaction-text").text = "Sell";

            Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

            if (playerInventory.FindItem(item.Item.item) == null)
            {
                transactionButton.EnableInClassList("disabled", true);
                transactionButton.EnableInClassList("main-button", false);
            }
            else
            {
                transactionButton.EnableInClassList("disabled", false);
                transactionButton.EnableInClassList("main-button", true);

                transactionButton.RegisterCallback<ClickEvent>(MarketSellEvent);
            }
        }
    }

    public void HandleMarketSlider(TransactionItem item)
    {
        quantSlider.UnregisterValueChangedCallback(QuantitySliderChanged);

        int quantity = -1;

        if (item.TransactionLocation == VulturaInstance.TransactionLocation.SUPPLY)
            quantity = item.Item.quantity;
        else
        {
            InventoryItem itemFound = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.FindItem(item.Item.item);
            
            if (itemFound != null)
                quantity = itemFound.quantity;
            else
                quantity = 0;
        }
            

        quantSlider.lowValue = 1;
        quantSlider.highValue = quantity;

        if (quantSlider.value < 0)
        {
            quantSlider.value = 1;
        }
        else
            quantSlider.value = quantSliderValue;

        quantityAmount.text = quantSliderValue.ToString();

        quantSlider.RegisterValueChangedCallback(QuantitySliderChanged);
    }

    // Display and write out the market item's details
    public void DisplayMarketElement(TransactionItem item)
    {
        //itemHeaderNoContent.style.display = DisplayStyle.None;

        itemHeaderContent.Q<Label>("item-name").text = item.Item.item.Name;
        (itemDescription as Label).text = item.Item.item.Description;
        (buyItemPrice as Label).text = "Buy 1 for $" + item.Item.buyPrice.ToString();
        (sellItemPrice as Label).text = "Sell 1 for $" + item.Item.sellPrice.ToString();

        itemHeaderNoContent.style.opacity = 0.0f;

        HandleMarketButton(item);
        HandleMarketSlider(item);
    }

    // When market element is selected while there is no element already there
    public void SelectMarketElement(TransactionItem item)
    {

        DisplayMarketElement(item);

    }

    // When a market element is already selected and we switch to a new one
    public void SwitchMarketElement(TransactionItem item)
    {
        DisplayMarketElement(item);
    }

    // When we want to deselect a market element
    public void DeselectMarketElement()
    {

        itemHeaderContent.style.opacity = 0.0f;
        itemDescription.style.opacity = 0.0f;
        buySellSection.style.opacity = 0.0f;
        pricingContainer.style.opacity = 0.0f;
        purchaseSection.style.opacity = 0.0f;
    }

    // Goes through rigorous checks before purchasing and adding the item to inventory
    private void PurchaseItem()
    {
        // Checks whether or not the item is set to be purchased. Simple check.
        if (currItem.Purchase)
        {
            // Sets the selected market to the only market you will buy from... supply
            Market selectedMarket = uiComponent.currentStation.market;
            int cost = currItem.Item.buyPrice * quantSliderValue;   // Sets the cost that the user must pay for the item(s).

            ExistsStruct itemExists = selectedMarket.ContainsItem(currItem.Item.item);

            // Checks whether or not the player can cover the cost as well as if the item exists
            if (VulturaInstance.playerMoney >= cost && itemExists.exists)
            {
                // Attempts to purchase the item. Will be null if it fails
                InventoryItem purchasedItem = selectedMarket.Purchase(currItem.Index, quantSliderValue);

                // Moves on if successful
                if (purchasedItem != null)
                {
                    // Checks whether or not the user can even add it to their inventory.
                    bool canAdd = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.CanAddToCargo(purchasedItem);

                    // If we can add it, subtract the player money and add it to their inventory. Else, put the item back.
                    if (canAdd)
                    {
                        VulturaInstance.playerMoney -= cost;

                        VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.AddToCargo(purchasedItem);
                    }
                    else
                    {
                        uiComponent.currentStation.market.Add(purchasedItem.item, purchasedItem.quantity);
                    }

                }
            }

            EventManager.TriggerEvent("inventory UI Refresh");
        }
    }

    private void SellItem()
    {
        if (!currItem.Purchase)
        {
            Market destinationMarket = null;
            int cost = currItem.Item.sellPrice * quantSliderValue;

            if (currItem.TransactionLocation == VulturaInstance.TransactionLocation.INVENTORY)
            {
                destinationMarket = uiComponent.currentStation.market;
            }
            else if (currItem.TransactionLocation == VulturaInstance.TransactionLocation.DEMAND)
            {
                destinationMarket = uiComponent.currentStation.demandMarket;
            }

            if (destinationMarket != null)
            {
                Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

                ExistsStruct hasItem = playerInventory.ContainsItem(currItem.Item.item);

                if (hasItem.exists)
                {
                    InventoryItem sellItem = playerInventory.PopAmount(hasItem.index, quantSliderValue);

                    VulturaInstance.playerMoney += cost;

                    destinationMarket.Add(currItem.Item.item, quantSliderValue);
                }
            }

            EventManager.TriggerEvent("inventory UI Refresh");
        }
    }

    // What?
    public void ClearData()
    {
        Debug.Log("Not Implemented!");
    }

    private void SetCurrItem(VisualElement element, TransactionItem item)
    {
        currentMarketSelection = element;
        currItem = item;
    }

    // -- EVENTS -- (Probably need an entire separate file for these)

    private void MarketPageEndTransition(TransitionEndEvent ev)
    {
        if (ev.stylePropertyNames.Contains("opacity"))
        {
            VisualElement element = (ev.currentTarget as VisualElement);
            if (element.style.opacity == 0.0f)
            {
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

    private void ItemHeaderNoContentEndTransition(TransitionEndEvent ev)
    {
        if (ev.stylePropertyNames.Contains("opacity"))
        {

            VisualElement element = (ev.currentTarget as VisualElement);

            if (element.style.opacity == 0.0f)
            {
                itemHeaderNoContent.style.display = DisplayStyle.None;

                itemHeaderContent.style.display = DisplayStyle.Flex;
                itemDescription.style.display = DisplayStyle.Flex;
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

    private void ItemHeaderContentEndTransition(TransitionEndEvent ev)
    {
        if (ev.stylePropertyNames.Contains("opacity"))
        {
            VisualElement element = (ev.currentTarget as VisualElement);

            if (element.style.opacity == 0.0f)
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

    private void MarketSupplyItemHover(PointerEnterEvent ev)
    {
        if (ev.currentTarget != currentMarketSelection)
        {
            VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
            element.style.backgroundColor = new StyleColor(marketActiveColor);
        }

    }

    private void MarketSupplyItemUnhover(PointerLeaveEvent ev)
    {
        if (ev.currentTarget != currentMarketSelection)
        {
            VisualElement element = (ev.currentTarget as VisualElement).Q<VisualElement>("market-button");
            element.style.backgroundColor = new StyleColor(marketInactiveColor);
        }

    }

    private void MarketSelectEvent(ClickEvent ev)
    {        
        bool deselect = false;

        if (currentMarketSelection == null)
            SelectMarketElement((ev.currentTarget as VisualElement).userData as TransactionItem);
        else
        {
            currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(marketInactiveColor);

            if (currentMarketSelection == ev.currentTarget)
            {
                deselect = true;
                SetCurrItem(null, null);
                DeselectMarketElement();
            }
            else
            {
                SwitchMarketElement((ev.currentTarget as VisualElement).userData as TransactionItem);
            }
        }

        if (!deselect)
        {
            VisualElement marketElement = ev.currentTarget as VisualElement; 
            SetCurrItem(marketElement, marketElement.userData as TransactionItem);
            currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(marketActiveColor);
        }

    }

    private void QuantitySliderChanged(ChangeEvent<int> ev)
    {
        quantSliderValue = ev.newValue;

        quantityAmount.text = ev.newValue.ToString();
    }

    private void MarketBuyEvent(ClickEvent ev)
    {
        PurchaseItem();
    }

    private void MarketSellEvent(ClickEvent ev)
    {
        SellItem();
    }

    private void MarketPagePointerUp(PointerUpEvent ev)
    {
        if (MasterOSManager.Instance.isDragging)
        {
            Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;
            int currentDraggedElement = (MasterOSManager.Instance.currentDraggedElement.userData as DragData).Index;
            ExistsStruct existsValue = uiComponent.currentStation.demandMarket.ContainsItem(playerInventory.itemList[currentDraggedElement].item); 
            BaseItem invItem = playerInventory.itemList[currentDraggedElement].item;
            int sellPrice = (int)Math.Floor(invItem.GalacticPrice * 0.75);
            VulturaInstance.TransactionLocation transactionLocation = VulturaInstance.TransactionLocation.INVENTORY;

            if (existsValue.exists)
            {
                sellPrice = uiComponent.currentStation.demandMarket.itemList[existsValue.index].sellPrice;
                transactionLocation = VulturaInstance.TransactionLocation.DEMAND;
            }

            TransactionItem item = new TransactionItem(transactionLocation, currentDraggedElement, false, new MarketItem(invItem, playerInventory.itemList[currentDraggedElement].quantity, 0, sellPrice, false));

            currItem = item;

            SelectMarketElement(item);
        }
    }
}
