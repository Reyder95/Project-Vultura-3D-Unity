using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public enum StationPage {
    HOME,
    CONTACT,
    MARKET,
    HAULING
}


enum SellMode {
    BUY,
    SELL
}

public class MarketSelectionData {
    public int elementIndex;
    public bool isMarket;

    public BaseItem itemReference;

    public MarketSelectionData(int elementIndex, bool isMarket, BaseItem itemReference)
    {
        this.elementIndex = elementIndex;
        this.isMarket = isMarket;
        this.itemReference = itemReference;
    }
}

public class StationUI : BaseOS
{

    // Events
    private UnityAction initListener;
    private UnityAction openListener;
    private UnityAction closeListener;

    // Misc. Stuff
    public PageStack pageStack;                    // The stack which handles what page is currently viewable.
    VisualElement screenBackground;         // The background of the entire UI
    VisualElement backButton;               // The button that goes back in the page stack
    VisualElement exitButton;               // The button that exits out of the GUI
    public StationPage nextPage;                   // The next page that we are moving to (useful for event transitions)
    public bool back = false;                      // Checks if we are moving back after a transition
    public BaseStation currentStation = null;      // The current station data

    // Page visual elements
    VisualElement homeVisualElement;        // The homepage
    VisualElement haulingVisualElement;     // The cargo hauling contracts page
    StationContact stationContactPage;
    StationMarketNew stationMarketPage;

    // Home page information
    VisualElement buttonStorage;            // The button for opening storage
    VisualElement buttonContacts;           // The button for opening contacts
    VisualElement buttonMarket;             // The button for opening market
    VisualElement buttonHauling;            // The button for opening hauling

    // Hauling page information
    VisualElement currHaulingContract = null;   // The current hauling contract that is selected
    VisualElement haulingHeaderContent;
    VisualElement noHaulingContent;
    VisualElement cargoDestination;
    VisualElement cargoItemContent;
    VisualElement haulingReward;

    public StationUI(string windowName, VisualElement screen): base(windowName, screen) {}

    public override void Awake()
    {
        initListener = new UnityAction(InitializeScreen);
        openListener = new UnityAction(OpenScreen);
        closeListener = new UnityAction(CloseScreen);
    }

    public override void OnEnable()
    {
        EventManager.StartListening("Market Changed", stationMarketPage.marketListener);
        EventManager.StartListening("station UI Event", initListener);
        EventManager.StartListening("station UI Open", openListener);
        EventManager.StartListening("station UI Close", closeListener);
    }

    public override void OnDisable()
    {
        EventManager.StopListening("Market Changed", stationMarketPage.marketListener);
        EventManager.StopListening("station UI Event", initListener);
        EventManager.StopListening("station UI Open", openListener);
        EventManager.StopListening("station UI Close", closeListener);
    }

    public override void Update() 
    {

        if (UIScreenManager.Instance.focusedScreen == screen)
        {
            screenBackground.style.opacity = 1.0f;
        }
        else
        {
            screenBackground.style.opacity = 0.2f;
        }

        // if (stationMarketPage != null) 
        //     stationMarketPage.HandleSellMode();
    }

    public override void InitializeScreen()
    {
        windowName = "station";
        base.InitializeScreen();

        pageStack = new PageStack();    // Initialize the page stack for this UI element

        screenBackground = screen.Q<VisualElement>("screen-background");

        // Grab all the pages and store them as visual elements
        homeVisualElement = screen.Q<VisualElement>("station-home");
        haulingVisualElement = screen.Q<VisualElement>("station-hauling");
        stationMarketPage = new StationMarketNew();
        stationContactPage = new StationContact();

        stationContactPage.SetTaggedReferences(screen, this);
        stationContactPage.SetCallbacks();

        stationMarketPage.SetTaggedReferences(screen, this);
        stationMarketPage.SetCallbacks();

        // Only display "home"
        haulingVisualElement.style.display = DisplayStyle.None;

        haulingVisualElement.style.opacity = new StyleFloat(0.0);

        backButton = screen.Q<VisualElement>("back-button");
        exitButton = screen.Q<VisualElement>("exit-button");
        buttonContacts = screen.Q<VisualElement>("button-contacts");
        buttonStorage = screen.Q<VisualElement>("button-storage");
        buttonMarket = screen.Q<VisualElement>("button-market");
        buttonHauling = screen.Q<VisualElement>("button-hauling");
        haulingHeaderContent = haulingVisualElement.Q<VisualElement>("hauling-header-content");
        noHaulingContent = haulingVisualElement.Q<VisualElement>("no-hauling-content");
        cargoDestination = haulingVisualElement.Q<VisualElement>("cargo-destination");
        cargoItemContent = haulingVisualElement.Q<VisualElement>("cargo-item-content");
        haulingReward = haulingVisualElement.Q<VisualElement>("hauling-reward");

        haulingHeaderContent.style.display = DisplayStyle.None;
        noHaulingContent.style.display = DisplayStyle.Flex;
        cargoDestination.style.display = DisplayStyle.None;
        cargoItemContent.style.display = DisplayStyle.None;
        haulingReward.style.display = DisplayStyle.None;

        haulingHeaderContent.style.opacity = 0.0f;
        noHaulingContent.style.opacity = 1.0f;
        cargoDestination.style.opacity = 0.0f;
        cargoItemContent.style.opacity = 0.0f;
        haulingReward.style.opacity = 0.0f;

        pageStack.Push(homeVisualElement);
        nextPage = StationPage.HOME;
        
        buttonContacts.RegisterCallback<ClickEvent>(ev => {
            nextPage = StationPage.CONTACT;
            homeVisualElement.style.opacity = new StyleFloat(0.0);
        });

        buttonHauling.RegisterCallback<ClickEvent>(ev => {
            nextPage = StationPage.HAULING;
            homeVisualElement.style.opacity = new StyleFloat(0.0);
        });

        buttonMarket.RegisterCallback<ClickEvent>(ev => {
            nextPage = StationPage.MARKET;
            homeVisualElement.style.opacity = new StyleFloat(0.0);
        });

        // Transition events
        homeVisualElement.RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);
                
                if (element.style.opacity == 0.0f)
                {
                    if (!back)
                        HandlePageTransition(DeterminePageElement());
                    else
                    {
                        pageStack.Pop();
                        pageStack.Top().style.opacity = new StyleFloat(1.0f);
                        back = false;
                    }
                }
            } 
        });

        haulingVisualElement.RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {
                    if (!back)
                        HandlePageTransition(DeterminePageElement());
                    else
                    {
                        pageStack.Pop();
                        pageStack.Top().style.opacity = new StyleFloat(1.0f);
                        back = false;
                    }
                }
            }
        });

        noHaulingContent.RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {
                    element.style.display = DisplayStyle.None;
                    haulingHeaderContent.style.display = DisplayStyle.Flex;
                    haulingHeaderContent.style.opacity = 1.0f;
                }
            }
        });

        haulingHeaderContent.RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 1.0f)
                {
                    cargoItemContent.style.display = DisplayStyle.Flex;
                    cargoItemContent.style.opacity = 1.0f;
                }
                else if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                    {
                        DisplayContract();
                        haulingHeaderContent.style.opacity = 1.0f;
                    }
                    else
                    {
                        haulingHeaderContent.style.display = DisplayStyle.None;
                        noHaulingContent.style.display = DisplayStyle.Flex;
                        noHaulingContent.style.opacity = 1.0f;
                    }

                }
            }
        });

        haulingVisualElement.Q<VisualElement>("cargo-item-content").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 1.0f)
                {
                    cargoDestination.style.display = DisplayStyle.Flex;
                    cargoDestination.style.opacity = 1.0f;
                }
                else if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                        cargoItemContent.style.opacity = 1.0f;
                }
            }
        });

        cargoDestination.RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 1.0f)
                {
                    haulingReward.style.display = DisplayStyle.Flex;
                    haulingReward.style.opacity = 1.0f;
                }
                else if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                        cargoDestination.style.opacity = 1.0f;
                }
            }
        });

        haulingReward.RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                        haulingReward.style.opacity = 1.0f;  
                }
            }
        });

        backButton.RegisterCallback<ClickEvent>(ev => {

            if (pageStack.CanGoBack())
            {
                back = true;
                pageStack.Top().style.opacity = 0.0f;
            }
        });

        exitButton.RegisterCallback<ClickEvent>(ev => {
            CloseScreen();
        });     

        try
        {
            UIScreenManager.Instance.SetFocusedScreen(screen);
        } catch (System.NullReferenceException ex)
        {
            Debug.Log("UIScreenManage is not available right now.");
        }

    }
 
    public override void OpenScreen()
    {
        base.OpenScreen();

        this.currentStation = StationManager.station;

        MasterOSManager.Instance.visualDict[windowName].style.display = DisplayStyle.Flex;
        LoadFacilities();
        stationContactPage.LoadContacts();
        LoadHauling();
        stationMarketPage.LoadMarket();
    }

    public override void CloseScreen()
    {
        base.CloseScreen();

        this.currentStation = null;
        stationMarketPage.ClearData();
        this.currHaulingContract = null;
        stationContactPage.ClearContact();
        UIScreenManager.Instance.RemoveScreen(screen);
        screen.style.display = DisplayStyle.None;
    }

    private void LoadHauling()
    {
        VisualElement haulingContainer = haulingVisualElement.Q<VisualElement>("hauling-left");

        for (int i = 0; i < currentStation.contracts.Count; i++)
        {
            VisualElement haulingItem = haulingContainer.ElementAt(i);
            haulingItem.Q<Label>("hauling-jump-distance").text = UnityEngine.Random.Range(5, 12).ToString() + " jumps away";
            haulingItem.Q<Label>("hauling-weight").text = currentStation.contracts[i].Items.currCargo.ToString() + " m3";

            haulingItem.userData = currentStation.contracts[i];

            haulingItem.RegisterCallback<ClickEvent>(ev => {
                if ((ev.currentTarget as VisualElement) != currHaulingContract)
                    SetCurrentHaulingContract((ev.currentTarget as VisualElement));
                else
                    SetCurrentHaulingContract(null);
            });

            haulingItem.style.visibility = Visibility.Visible;
        }
    }

    private void LoadFacilities()
    { 
        VisualElement facilityContainer = homeVisualElement.Q<VisualElement>("home-body-right");

        for (int i = 0; i < currentStation.facilities.Count; i++)
        {
            VisualElement facilityItem = facilityContainer.ElementAt(i);
            facilityItem.Q<Label>("facility-name").text = currentStation.facilities[i].facilityName;
            
            if (currentStation.facilities[i].demand)
                facilityItem.Q<Label>("facility-status").text = "<color=\"red\">Demand</color>";
            else
                facilityItem.Q<Label>("facility-status").text = "<color=\"green\">OK</color>";

            facilityItem.style.visibility = Visibility.Visible;
        }
    }

    private void SetCurrentHaulingContract(VisualElement haulingContract)
    {
        if (haulingContract != null)
        {
            if (currHaulingContract != null)
            {
                currHaulingContract.EnableInClassList("active-facility-button", false);

                haulingHeaderContent.style.opacity = 0.0f;
                cargoItemContent.style.opacity = 0.0f;
                cargoDestination.style.opacity = 0.0f;
                haulingReward.style.opacity = 0.0f;

                currHaulingContract = haulingContract;

                currHaulingContract.EnableInClassList("active-facility-button", true);
            }
            else
            {
                currHaulingContract = haulingContract;

                currHaulingContract.EnableInClassList("active-facility-button", true);

                DisplayContract();
            }
        }
        else
        {
            currHaulingContract.EnableInClassList("active-facility-button", false);
            currHaulingContract = null;

            DisplayContract();
        }

        
    }

    private void DisplayContract()
    {
        if (currHaulingContract != null)
        {
            Contract currContract = currHaulingContract.userData as Contract;

            haulingHeaderContent.Q<Label>("hauling-jump-distance").text = currHaulingContract.Q<Label>("hauling-jump-distance").text;
            haulingHeaderContent.Q<Label>("hauling-weight").text = currContract.Items.currCargo + " m3";

            noHaulingContent.style.opacity = 0.0f;

            for (int i = 0; i < 4; i++)
            {
                if (currContract.Items.itemList.Count > i)
                {
                    VisualElement currHaulingVisualElement = haulingVisualElement.Q<VisualElement>("cargo-item-content").ElementAt(i);
                    currHaulingVisualElement.Q<Label>("hauling-item-name").text = currContract.Items.itemList[i].quantity.ToString() + " " + currContract.Items.itemList[i].item.Name;
                    haulingVisualElement.Q<Label>("cargo-destination").text = currContract.Destination;
                    currHaulingVisualElement.style.visibility = Visibility.Visible;
                }
                else
                {
                    cargoItemContent.ElementAt(i).style.visibility = Visibility.Hidden;
                }

            }

            (haulingReward as Label).text = "$" + currContract.Reward.ToString();
        }
        else
        {
            haulingHeaderContent.style.opacity = 0.0f;
            cargoItemContent.style.opacity = 0.0f;
            cargoDestination.style.opacity = 0.0f;
            haulingReward.style.opacity = 0.0f;
        }
    }

    public VisualElement DeterminePageElement()
    {
        if (nextPage == StationPage.HOME)
            return homeVisualElement;
        else if (nextPage == StationPage.CONTACT)
            return stationContactPage.ReturnPage();
        else if (nextPage == StationPage.HAULING)
            return haulingVisualElement;
        else if (nextPage == StationPage.MARKET)
            return stationMarketPage.ReturnPage();

        return null;
    }

    public void HandlePageTransition(VisualElement newPageElement)
    {   
        Debug.Log(nextPage.ToString());
        pageStack.Push(newPageElement);
        
        newPageElement.style.opacity = new StyleFloat(1.0f);
    }

}
