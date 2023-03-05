using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

enum StationPage {
    HOME,
    CONTACT,
    MARKET,
    HAULING
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

    // Visual Tree Stuff
    public VisualTreeAsset marketItemAsset;

    PageStack pageStack;                    // The stack which handles what page is currently viewable.

    VisualElement homeVisualElement;        // The homepage
    VisualElement haulingVisualElement;     // The cargo hauling contracts page
    VisualElement contactsVisualElement;    // The contacts page
    VisualElement marketVisualElement;      // The market page

    VisualElement screenBackground;

    VisualElement backButton;               // The button that goes back in the page stack
    VisualElement exitButton;               // The button that exits out of the GUI

    StationPage nextPage;

    private UnityAction marketListener;
    private UnityAction initListener;
    private UnityAction openListener;
    private UnityAction closeListener;

    bool back = false;

    BaseStation currentStation = null;

    // Contact stuff
    VisualElement currContact = null;
    ConversationStack convoStack;

    // Hauling stuff
    VisualElement currHaulingContract = null;

    // Market stuff
    ListView marketList;
    ListView demandList;
    VisualElement currentMarketSelection;
    Market marketElement;
    int quantSliderValue = -1;

    int marketHovered = -1;
    bool isMarketHovered = false;

    private void Awake()
    {
        marketListener = new UnityAction(RefreshMarket);
        initListener = new UnityAction(InitializeScreen);
        openListener = new UnityAction(OpenScreen);
        closeListener = new UnityAction(CloseScreen);
    }

    private void OnEnable()
    {
        EventManager.StartListening("Market Changed", marketListener);
        EventManager.StartListening("station UI Event", initListener);
        EventManager.StartListening("station UI Open", openListener);
        EventManager.StartListening("station UI Close", closeListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Market Changed", marketListener);
        EventManager.StopListening("station UI Event", initListener);
        EventManager.StopListening("station UI Open", openListener);
        EventManager.StopListening("station UI Close", closeListener);
    }

    void Update() 
    {

        if (UIScreenManager.Instance.focusedScreen == screen)
        {
            screenBackground.style.opacity = 1.0f;
        }
        else
        {
            screenBackground.style.opacity = 0.2f;
        }
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
        contactsVisualElement = screen.Q<VisualElement>("station-contacts");
        marketVisualElement = screen.Q<VisualElement>("station-market");


        // Only display "home"
        haulingVisualElement.style.display = DisplayStyle.None;
        contactsVisualElement.style.display = DisplayStyle.None;
        marketVisualElement.style.display = DisplayStyle.None;

        haulingVisualElement.style.opacity = new StyleFloat(0.0);
        contactsVisualElement.style.opacity = new StyleFloat(0.0);
        marketVisualElement.style.opacity = new StyleFloat(0.0);

        pageStack.Push(homeVisualElement);
        nextPage = StationPage.HOME;

        backButton = screen.Q<VisualElement>("back-button");
        exitButton = screen.Q<VisualElement>("exit-button");
        
        screen.Q<VisualElement>("button-contacts").RegisterCallback<ClickEvent>(ev => {
            nextPage = StationPage.CONTACT;
            homeVisualElement.style.opacity = new StyleFloat(0.0);
        });

        screen.Q<VisualElement>("button-hauling").RegisterCallback<ClickEvent>(ev => {
            nextPage = StationPage.HAULING;
            homeVisualElement.style.opacity = new StyleFloat(0.0);
        });

        screen.Q<VisualElement>("button-market").RegisterCallback<ClickEvent>(ev => {
            nextPage = StationPage.MARKET;
            homeVisualElement.style.opacity = new StyleFloat(0.0);
        });

        contactsVisualElement.Q<VisualElement>("contact-content-overlay").style.opacity = 0.0f;
        contactsVisualElement.Q<VisualElement>("contact-header-content").style.opacity = 0.0f;

        haulingVisualElement.Q<VisualElement>("hauling-header-content").style.display = DisplayStyle.None;
        haulingVisualElement.Q<VisualElement>("no-hauling-content").style.display = DisplayStyle.Flex;
        haulingVisualElement.Q<VisualElement>("cargo-destination").style.display = DisplayStyle.None;
        haulingVisualElement.Q<VisualElement>("cargo-item-content").style.display = DisplayStyle.None;
        haulingVisualElement.Q<VisualElement>("hauling-reward").style.display = DisplayStyle.None;


        haulingVisualElement.Q<VisualElement>("hauling-header-content").style.opacity = 0.0f;
        haulingVisualElement.Q<VisualElement>("no-hauling-content").style.opacity = 1.0f;
        haulingVisualElement.Q<VisualElement>("cargo-destination").style.opacity = 0.0f;
        haulingVisualElement.Q<VisualElement>("cargo-item-content").style.opacity = 0.0f;
        haulingVisualElement.Q<VisualElement>("hauling-reward").style.opacity = 0.0f;

        marketVisualElement.Q<VisualElement>("item-header-content").style.display = DisplayStyle.None;
        marketVisualElement.Q<VisualElement>("item-header-no-content").style.display = DisplayStyle.Flex;
        marketVisualElement.Q<VisualElement>("item-description").style.display = DisplayStyle.None;
        marketVisualElement.Q<VisualElement>("buy-sell-section").style.display = DisplayStyle.None;
        marketVisualElement.Q<VisualElement>("pricing-container").style.display = DisplayStyle.None;
        marketVisualElement.Q<VisualElement>("purchase-section").style.display = DisplayStyle.None;

        marketVisualElement.Q<VisualElement>("item-header-content").style.opacity = 0.0f;
        marketVisualElement.Q<VisualElement>("item-header-no-content").style.opacity = 1.0f;
        marketVisualElement.Q<VisualElement>("item-description").style.opacity = 0.0f;
        marketVisualElement.Q<VisualElement>("buy-sell-section").style.opacity = 0.0f;
        marketVisualElement.Q<VisualElement>("pricing-container").style.opacity = 0.0f;
        marketVisualElement.Q<VisualElement>("purchase-section").style.opacity = 0.0f;

        contactsVisualElement.Q<VisualElement>("contact-content-overlay").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {
                    if ((ev.currentTarget as VisualElement).userData != null)
                    {
                        if ((ev.currentTarget as VisualElement).userData is Conversation)
                        {
                            convoStack.Push(((ev.currentTarget as VisualElement).userData as Conversation));
                            DisplayConvo();
                        }
                        else if (((ev.currentTarget as VisualElement).userData as string) == "back")
                        {
                            convoStack.Pop();

                            if (convoStack.IsEmpty())
                            {
                                convoStack = null;
                                SetCurrentContact(null);
                            }  

                            DisplayConvo();
                        }
                    }
                }
            }
        });

        contactsVisualElement.Q<VisualElement>("contact-no-selection-content").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {

                    VisualElement contactRight = contactsVisualElement.Q<VisualElement>("contacts-right");
                    contactsVisualElement.Q<VisualElement>("contact-no-selection-content").style.display = DisplayStyle.None;
                    contactsVisualElement.Q<VisualElement>("contact-header-content").style.display = DisplayStyle.Flex;
                    contactRight.Q<Label>("contact-name").text = (currContact.userData as Contact).Name;
                    contactRight.Q<Label>("contact-role").text = VulturaInstance.enumStringParser((currContact.userData as Contact).Type.ToString());
                    contactsVisualElement.Q<VisualElement>("contact-header-content").style.opacity = 1.0f;
                }
            }
        });

        contactsVisualElement.Q<VisualElement>("contact-header-content").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 1.0f)
                {
                    InitializeConversationStack();
                }
                else if (element.style.opacity == 0.0f)
                {
                    contactsVisualElement.Q<VisualElement>("contact-no-selection-content").style.display = DisplayStyle.Flex;
                    contactsVisualElement.Q<VisualElement>("contact-no-selection-content").style.opacity = 1.0f;
                }
            }
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

        contactsVisualElement.RegisterCallback<TransitionEndEvent>(ev => {
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

        marketVisualElement.RegisterCallback<TransitionEndEvent>(ev => {
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

        haulingVisualElement.Q<VisualElement>("no-hauling-content").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {
                    element.style.display = DisplayStyle.None;
                    haulingVisualElement.Q<VisualElement>("hauling-header-content").style.display = DisplayStyle.Flex;
                    haulingVisualElement.Q<VisualElement>("hauling-header-content").style.opacity = 1.0f;
                }
            }
        });

        haulingVisualElement.Q<VisualElement>("hauling-header-content").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 1.0f)
                {
                    haulingVisualElement.Q<VisualElement>("cargo-item-content").style.display = DisplayStyle.Flex;
                    haulingVisualElement.Q<VisualElement>("cargo-item-content").style.opacity = 1.0f;
                }
                else if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                    {
                        DisplayContract();
                        haulingVisualElement.Q<VisualElement>("hauling-header-content").style.opacity = 1.0f;
                    }
                    else
                    {
                        haulingVisualElement.Q<VisualElement>("hauling-header-content").style.display = DisplayStyle.None;
                        haulingVisualElement.Q<VisualElement>("no-hauling-content").style.display = DisplayStyle.Flex;
                        haulingVisualElement.Q<VisualElement>("no-hauling-content").style.opacity = 1.0f;
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
                    haulingVisualElement.Q<VisualElement>("cargo-destination").style.display = DisplayStyle.Flex;
                    haulingVisualElement.Q<VisualElement>("cargo-destination").style.opacity = 1.0f;
                }
                else if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                        haulingVisualElement.Q<VisualElement>("cargo-item-content").style.opacity = 1.0f;
                }
            }
        });

        haulingVisualElement.Q<VisualElement>("cargo-destination").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 1.0f)
                {
                    haulingVisualElement.Q<VisualElement>("hauling-reward").style.display = DisplayStyle.Flex;
                    haulingVisualElement.Q<VisualElement>("hauling-reward").style.opacity = 1.0f;
                }
                else if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                        haulingVisualElement.Q<VisualElement>("cargo-destination").style.opacity = 1.0f;
                }
            }
        });

        haulingVisualElement.Q<VisualElement>("hauling-reward").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                        haulingVisualElement.Q<VisualElement>("hauling-reward").style.opacity = 1.0f;  
                }
            }
        });

        marketVisualElement.Q<VisualElement>("item-header-no-content").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {
                    marketVisualElement.Q<VisualElement>("item-header-no-content").style.display = DisplayStyle.None;
                    marketVisualElement.Q<VisualElement>("item-description").style.display = DisplayStyle.Flex;
                    marketVisualElement.Q<VisualElement>("item-header-content").style.display = DisplayStyle.Flex;
                    marketVisualElement.Q<VisualElement>("buy-sell-section").style.display = DisplayStyle.Flex;
                    marketVisualElement.Q<VisualElement>("pricing-container").style.display = DisplayStyle.Flex;
                    marketVisualElement.Q<VisualElement>("purchase-section").style.display = DisplayStyle.Flex;
                    marketVisualElement.Q<VisualElement>("item-header-content").style.opacity = 1.0f;
                    marketVisualElement.Q<VisualElement>("item-description").style.opacity = 1.0f;
                    marketVisualElement.Q<VisualElement>("buy-sell-section").style.opacity = 1.0f;
                    marketVisualElement.Q<VisualElement>("pricing-container").style.opacity = 1.0f;
                    marketVisualElement.Q<VisualElement>("purchase-section").style.opacity = 1.0f;
                }
            }
        });

        marketVisualElement.Q<VisualElement>("item-header-content").RegisterCallback<TransitionEndEvent>(ev => {
            if (ev.stylePropertyNames.Contains("opacity"))
            {
                VisualElement element = (ev.currentTarget as VisualElement);

                if (element.style.opacity == 0.0f)
                {
                    if (currentMarketSelection != null)
                    {
                        DisplayMarket();
                        marketVisualElement.Q<VisualElement>("item-header-content").style.opacity = 1.0f;
                        marketVisualElement.Q<VisualElement>("item-description").style.opacity = 1.0f;
                        marketVisualElement.Q<VisualElement>("buy-sell-section").style.opacity = 1.0f;
                        marketVisualElement.Q<VisualElement>("pricing-container").style.opacity = 1.0f;
                        marketVisualElement.Q<VisualElement>("purchase-section").style.opacity = 1.0f;
                    }
                    else
                    {
                        marketVisualElement.Q<VisualElement>("item-header-content").style.display = DisplayStyle.None;
                        marketVisualElement.Q<VisualElement>("item-description").style.display = DisplayStyle.None;
                        marketVisualElement.Q<VisualElement>("buy-sell-section").style.display = DisplayStyle.None;
                        marketVisualElement.Q<VisualElement>("pricing-container").style.display = DisplayStyle.None;
                        marketVisualElement.Q<VisualElement>("purchase-section").style.display = DisplayStyle.None;

                        marketVisualElement.Q<VisualElement>("item-header-no-content").style.display = DisplayStyle.Flex;
                        marketVisualElement.Q<VisualElement>("item-header-no-content").style.opacity = 1.0f;
                    }
                }
            }
        });

        backButton.RegisterCallback<ClickEvent>(ev => {

            if (pageStack.CanGoBack())
            {
                back = true;
                pageStack.Top().style.opacity = new StyleFloat(0.0);
            }
        });

        exitButton.RegisterCallback<ClickEvent>(ev => {
            CloseScreen();
        });

        VisualElement convoChoices = contactsVisualElement.Q<VisualElement>("convo-choices");

        for (int i = 0; i < 3; i++)
        {
            convoChoices.ElementAt(i).style.opacity = 0.0f;
        }
        

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
        LoadContacts();
        LoadHauling();
        LoadMarket();
    }

    public override void CloseScreen()
    {
        base.CloseScreen();

        this.currentStation = null;
        this.currentMarketSelection = null;
        this.currHaulingContract = null;
        this.currContact = null;
        marketList = null;
        demandList = null;
        UIScreenManager.Instance.RemoveScreen(screen);
        MasterOSManager.Instance.visualDict[windowName].style.display = DisplayStyle.None;
    }

    private void LoadMarket()
    {
        marketList = marketVisualElement.Q<ListView>("sale-market");

        Func<VisualElement> makeItemMarket = () => marketItemAsset.Instantiate();

        Action<VisualElement, int> bindItemMarket = (e, i) => {
            e.Q<Label>("item-name").text = currentStation.market.itemList[i].item.Name + " / " + currentStation.market.itemList[i].quantity.ToString() + "x";
            e.Q<Label>("item-price").text = "Buy: $" + currentStation.market.itemList[i].buyPrice.ToString();

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

            MarketSelectionData newData = new MarketSelectionData(i, true, currentStation.market.itemList[i].item);
            e.userData = newData;

            e.RegisterCallback<ClickEvent>(ev => {
                
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
        marketList.itemsSource = currentStation.market.itemList;

        demandList = marketVisualElement.Q<ListView>("sale-demand");

        Func<VisualElement> makeItemDemand = () => marketItemAsset.Instantiate();

        Action<VisualElement, int> bindItemDemand = (e, i) => {
            e.Q<Label>("item-name").text = currentStation.demandMarket.itemList[i].item.Name;
            e.Q<Label>("item-price").text = "Sell: $" + currentStation.demandMarket.itemList[i].sellPrice.ToString();

            MarketSelectionData newData = new MarketSelectionData(i, false, currentStation.demandMarket.itemList[i].item);
            e.userData = newData;

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
        demandList.itemsSource = currentStation.demandMarket.itemList;
    }

    private void SetCurrentMarketItem(VisualElement marketItem)
    {
        if (marketItem != null)
        {
            if (currentMarketSelection != null)
            {

                currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(104, 124,227, 51));

                marketVisualElement.Q<VisualElement>("item-header-content").style.opacity = 0.0f;
                marketVisualElement.Q<VisualElement>("item-description").style.opacity = 0.0f;
                marketVisualElement.Q<VisualElement>("buy-sell-section").style.opacity = 0.0f;
                marketVisualElement.Q<VisualElement>("pricing-container").style.opacity = 0.0f;
                marketVisualElement.Q<VisualElement>("purchase-section").style.opacity = 0.0f;

                currentMarketSelection = marketItem;

                currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));
            }
            else
            {
                currentMarketSelection = marketItem;

                currentMarketSelection.Q<VisualElement>("market-button").style.backgroundColor = new StyleColor(new Color32(176, 185, 232, 51));

                DisplayMarket();
            }
        }
        else 
        {
            currentMarketSelection = null;
            quantSliderValue = -1;

            DisplayMarket();
        }
    }

    private void RefreshMarket()
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
                    marketElement = currentStation.market;
                else
                    marketElement = currentStation.demandMarket;

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

    void BuyItemCallback(ClickEvent ev)
    {
        BuyItem(marketElement, (currentMarketSelection.userData as MarketSelectionData).elementIndex, quantSliderValue);
    }

    void SellItemDemandCallback(ClickEvent ev)
    {
        SellItemDemand((currentMarketSelection.userData as MarketSelectionData).elementIndex, quantSliderValue);
    }

    private void DisplayMarket()
    {
        if (currentMarketSelection != null)
        {
            MarketSelectionData marketSelectionData = currentMarketSelection.userData as MarketSelectionData;

            Market marketElement = null;

            if (marketSelectionData.isMarket)
                marketElement = currentStation.market;
            else
                marketElement = currentStation.demandMarket;

            marketVisualElement.Q<VisualElement>("item-header-content").Q<Label>("item-name").text = marketElement.itemList[marketSelectionData.elementIndex].item.Name;
            marketVisualElement.Q<Label>("item-description").text = marketElement.itemList[marketSelectionData.elementIndex].item.Description;
            marketVisualElement.Q<Label>("buy-item-price").text = "Buy 1 for $" + marketElement.itemList[marketSelectionData.elementIndex].buyPrice.ToString();
            marketVisualElement.Q<Label>("sell-item-price").text = "Sell 1 for $" + marketElement.itemList[marketSelectionData.elementIndex].sellPrice.ToString();

            SliderInt quantSlider = marketVisualElement.Q<SliderInt>("quantity-slider");

            if (marketSelectionData.isMarket)
            {
                marketVisualElement.Q<Label>("transaction-text").text = "Buy";

                marketVisualElement.Q<VisualElement>("transaction-button").UnregisterCallback<ClickEvent>(BuyItemCallback);

                marketVisualElement.Q<VisualElement>("transaction-button").RegisterCallback<ClickEvent>(BuyItemCallback);

                marketVisualElement.Q<VisualElement>("transaction-button").EnableInClassList("disabled", false);
                marketVisualElement.Q<VisualElement>("transaction-button").EnableInClassList("main-button", true);

                quantSlider.lowValue = 1;
                quantSlider.highValue = marketElement.itemList[marketSelectionData.elementIndex].quantity;

                if (quantSliderValue < 0)
                    quantSlider.value = 1;
                else
                    quantSlider.value = quantSliderValue;

                quantSlider.RegisterValueChangedCallback(ev => {
                    quantSliderValue = ev.newValue;
                    marketVisualElement.Q<Label>("quantity-amount").text = ev.newValue.ToString();
                    marketVisualElement.Q<Label>("purchase-currency").text = "$" + (marketElement.itemList[marketSelectionData.elementIndex].buyPrice * ev.newValue).ToString();
                });
            }
            else
            {
                Inventory playerInventory = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo;

                BaseItem selectedItem = marketElement.itemList[marketSelectionData.elementIndex].item;

                InventoryItem playerInvItem = playerInventory.FindItem(selectedItem);

                marketVisualElement.Q<Label>("transaction-text").text = "Sell";

                marketVisualElement.Q<VisualElement>("transaction-button").RegisterCallback<ClickEvent>(ev => {
                    
                });

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

                    marketVisualElement.Q<VisualElement>("transaction-button").EnableInClassList("disabled", false);
                    marketVisualElement.Q<VisualElement>("transaction-button").EnableInClassList("main-button", true);
                    
                    marketVisualElement.Q<VisualElement>("transaction-button").RegisterCallback<ClickEvent>(ev => {
                        SellItemDemand(marketSelectionData.elementIndex, quantSlider.value);
                    });
                }
                else
                {
                    quantSlider.lowValue = 0;
                    quantSlider.highValue = 0;
                    quantSlider.value = 0;

                    quantSlider.UnregisterValueChangedCallback(ev => {
                        HandleQuantSlider(ev, marketElement, marketSelectionData.elementIndex);
                    });

                    marketVisualElement.Q<Label>("quantity-amount").text = "0";
                    marketVisualElement.Q<Label>("purchase-currency").text = "N/A";

                    marketVisualElement.Q<VisualElement>("transaction-button").EnableInClassList("disabled", true);
                    marketVisualElement.Q<VisualElement>("transaction-button").EnableInClassList("main-button", false);

                    marketVisualElement.Q<VisualElement>("transaction-button").UnregisterCallback<ClickEvent>(ev => {
                        SellItemDemand(marketSelectionData.elementIndex, quantSlider.value);
                    });
                }

            }

            marketVisualElement.Q<VisualElement>("item-header-no-content").style.opacity = 0.0f;

        } 
        else 
        {
            marketVisualElement.Q<VisualElement>("item-header-content").style.opacity = 0.0f;
            marketVisualElement.Q<VisualElement>("item-description").style.opacity = 0.0f;
            marketVisualElement.Q<VisualElement>("buy-sell-section").style.opacity = 0.0f;
            marketVisualElement.Q<VisualElement>("pricing-container").style.opacity = 0.0f;
            marketVisualElement.Q<VisualElement>("purchase-section").style.opacity = 0.0f;
        }
        
    }

    private void HandleQuantSlider(ChangeEvent<int> ev, Market marketElement, int elementIndex)
    {
        marketVisualElement.Q<Label>("quantity-amount").text = ev.newValue.ToString();
        marketVisualElement.Q<Label>("purchase-currency").text = "$" + (marketElement.itemList[elementIndex].sellPrice * ev.newValue).ToString();
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
        Market demandMarket = currentStation.demandMarket;

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

                currentStation.InsertIntoStockpile(soldItem, quantity);

                return true;
            }
        }

        return false;
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

                haulingVisualElement.Q<VisualElement>("hauling-header-content").style.opacity = 0.0f;
                haulingVisualElement.Q<VisualElement>("cargo-item-content").style.opacity = 0.0f;
                haulingVisualElement.Q<VisualElement>("cargo-destination").style.opacity = 0.0f;
                haulingVisualElement.Q<VisualElement>("hauling-reward").style.opacity = 0.0f;

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

            haulingVisualElement.Q<VisualElement>("hauling-header-content").Q<Label>("hauling-jump-distance").text = currHaulingContract.Q<Label>("hauling-jump-distance").text;
            haulingVisualElement.Q<VisualElement>("hauling-header-content").Q<Label>("hauling-weight").text = currContract.Items.currCargo + " m3";

            haulingVisualElement.Q<VisualElement>("no-hauling-content").style.opacity = 0.0f;

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
                    haulingVisualElement.Q<VisualElement>("cargo-item-content").ElementAt(i).style.visibility = Visibility.Hidden;
                }

            }

            haulingVisualElement.Q<Label>("hauling-reward").text = "$" + currContract.Reward.ToString();
        }
        else
        {
            haulingVisualElement.Q<VisualElement>("hauling-header-content").style.opacity = 0.0f;
            haulingVisualElement.Q<VisualElement>("cargo-item-content").style.opacity = 0.0f;
            haulingVisualElement.Q<VisualElement>("cargo-destination").style.opacity = 0.0f;
            haulingVisualElement.Q<VisualElement>("hauling-reward").style.opacity = 0.0f;
        }
    }

    private void SetCurrentContact(VisualElement contact)
    {
        if (contact != null)
        {
            if (currContact != null)
                currContact.EnableInClassList("active-facility-button", false);
            currContact = contact;

            Contact myContact = (contact.userData as Contact);

            contactsVisualElement.Q<VisualElement>("contact-no-selection-content").style.opacity = 0.0f;
            contact.EnableInClassList("active-facility-button", true);
        }
        else
        {
            
            currContact.EnableInClassList("active-facility-button", false);
            currContact = null;

            contactsVisualElement.Q<VisualElement>("contact-header-content").style.opacity = 0.0f;

            contactsVisualElement.Q<VisualElement>("contact-content-overlay").style.opacity = 0.0f;
            VisualElement convoChoices = contactsVisualElement.Q<VisualElement>("convo-choices");
            for (int i = 0; i < 3; i++)
            {
                convoChoices.ElementAt(i).style.opacity = 0.0f;
            }
        }
    }

    private void LoadContacts()
    {
        VisualElement contactContainer = contactsVisualElement.Q<VisualElement>("contacts-left");

        VisualElement headItem = contactContainer.ElementAt(0);
        headItem.Q<Label>("contact-name").text = currentStation.stationHead.Name;
        headItem.Q<Label>("contact-role").text = VulturaInstance.enumStringParser(currentStation.stationHead.Type.ToString());
        headItem.userData = currentStation.stationHead;
        headItem.style.visibility = Visibility.Visible;

        headItem.RegisterCallback<ClickEvent>(ev => {
            if (currContact == (ev.currentTarget as VisualElement))
                SetCurrentContact(null);
            else
                SetCurrentContact((ev.currentTarget as VisualElement));
        });

        for (int i = 0; i < currentStation.contacts.Count; i++)
        {
            VisualElement contactItem = contactContainer.ElementAt(i+1);
            contactItem.Q<Label>("contact-name").text = currentStation.contacts[i].Name;
            contactItem.Q<Label>("contact-role").text = VulturaInstance.enumStringParser(currentStation.contacts[i].Type.ToString());
            contactItem.userData = currentStation.contacts[i];
            contactItem.style.visibility = Visibility.Visible;

            contactItem.RegisterCallback<ClickEvent>(ev => {
                if (currContact == (ev.currentTarget as VisualElement))
                    SetCurrentContact(null);
                else
                    SetCurrentContact((ev.currentTarget as VisualElement));
            });
        }
    }

    private VisualElement DeterminePageElement()
    {
        if (nextPage == StationPage.HOME)
            return homeVisualElement;
        else if (nextPage == StationPage.CONTACT)
            return contactsVisualElement;
        else if (nextPage == StationPage.HAULING)
            return haulingVisualElement;
        else if (nextPage == StationPage.MARKET)
            return marketVisualElement;

        return null;
    }

    private void HandlePageTransition(VisualElement newPageElement)
    {   
        pageStack.Push(newPageElement);
        
        newPageElement.style.opacity = new StyleFloat(1.0f);
    }

    // Initialize the beginning of the conversation stack baseed on which contact was selected
    public void InitializeConversationStack()
    {
        if (currContact == null)
        {
            convoStack = null;
        }
        else
        {
            convoStack = new ConversationStack();
            convoStack.Push((currContact.userData as Contact).Conversation);
        }

        DisplayConvo();
    }

    // Displays the convo based on the conversation at the top
    public void DisplayConvo()
    {
        // If there is no convo stack, make it show the "no convostack" visual element
        if (convoStack == null)
        {
            contactsVisualElement.Q<VisualElement>("contact-content-overlay").style.display = DisplayStyle.None;
            contactsVisualElement.Q<VisualElement>("convo-choices").style.display = DisplayStyle.None;

            contactsVisualElement.Q<VisualElement>("contact-right-content").style.display = DisplayStyle.None;
        }
        else
        {

            // Set the paragraph content and show the prompt
            contactsVisualElement.Q<Label>("contact-content").text = convoStack.Top().Prompt;

            VisualElement choicesContainer = contactsVisualElement.Q<VisualElement>("convo-choices");

            // For up to 3 responses, display each response
            for (int i = 0; i < 3; i++)
            {
                Conversation topConvo = convoStack.Top();

                if (i < topConvo.Responses.Count)
                {
                    BaseResponse currResponse = convoStack.Top().Responses[i];
                    choicesContainer.ElementAt(i).Q<Label>("text-content").text = convoStack.Top().Responses[i].Prompt;
                    choicesContainer.ElementAt(i).userData = currResponse;
                    choicesContainer.ElementAt(i).UnregisterCallback<ClickEvent>(ResponseClick);
                    choicesContainer.ElementAt(i).RegisterCallback<ClickEvent>(ResponseClick);
                    choicesContainer.ElementAt(i).style.visibility = Visibility.Visible;
                    choicesContainer.ElementAt(i).style.opacity = 1.0f;
                }
                else
                {
                    choicesContainer.ElementAt(i).style.visibility = Visibility.Hidden;
                }
                
            }

            choicesContainer.style.display = DisplayStyle.Flex;
            contactsVisualElement.Q<VisualElement>("contact-content-overlay").style.display = DisplayStyle.Flex;
            contactsVisualElement.Q<VisualElement>("contact-content-overlay").style.opacity = 1.0f;

            contactsVisualElement.Q<VisualElement>("contact-right-content").style.display = DisplayStyle.Flex;


        }
    }

    // When a conversation response is clicked, handle it
    public void ResponseClick(ClickEvent ev)
    {
        VisualElement contactElement = ev.currentTarget as VisualElement;
        HandleResponses((contactElement.userData as BaseResponse));
    }

    // Add the conversation to the top of the stack (based on the response selected)
    public void HandleResponses(BaseResponse response)
    {
        if (response.Type == VulturaInstance.ResponseType.Basic)
        {
            BasicResponse currResponse = response as BasicResponse;
            if (currResponse.GoBack)
            {
                contactsVisualElement.Q<VisualElement>("contact-content-overlay").userData = "back";
                contactsVisualElement.Q<VisualElement>("contact-content-overlay").style.opacity = 0.0f;

                VisualElement choicesContainer = contactsVisualElement.Q<VisualElement>("convo-choices");

                for (int i = 0; i < 3; i++)
                {
                    choicesContainer.ElementAt(i).style.opacity = 0.0f;
                }
            }
            else
            {
                if (currResponse.Conversation != null)
                {
                    contactsVisualElement.Q<VisualElement>("contact-content-overlay").userData = currResponse.Conversation;
                    contactsVisualElement.Q<VisualElement>("contact-content-overlay").style.opacity = 0.0f;

                    VisualElement choicesContainer = contactsVisualElement.Q<VisualElement>("convo-choices");

                    for (int i = 0; i < 3; i++)
                    {
                        choicesContainer.ElementAt(i).style.opacity = 0.0f;
                    }

                    //convoStack.Push(currResponse.Conversation);
                    //convoStack.DisplayConvoStack();
                } 
            }
            
            //DisplayConvo();
        }
    }

}
