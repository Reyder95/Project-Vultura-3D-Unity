using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

enum StationPage {
    HOME,
    CONTACT,
    MARKET,
    HAULING
}

public class ConversationTransitionData {
    Conversation convo;
    bool back;
}

public class StationUI : MonoBehaviour
{
    PageStack pageStack;                    // The stack which handles what page is currently viewable.

    VisualElement rootVisualElement;        // The root visual element for the entire station GUI

    VisualElement homeVisualElement;        // The homepage
    VisualElement haulingVisualElement;     // The cargo hauling contracts page
    VisualElement contactsVisualElement;    // The contacts page

    VisualElement backButton;               // The button that goes back in the page stack
    VisualElement exitButton;               // The button that exits out of the GUI

    StationPage nextPage;

    bool back = false;

    BaseStation currentStation = null;

    // Contact stuff
    VisualElement currContact = null;
    ConversationStack convoStack;

    // Hauling stuff
    VisualElement currHaulingContract = null;

    public void InitializeStation(BaseStation station)
    {
        this.gameObject.SetActive(true);
        this.currentStation = station;

        InitializeAllPages();
        LoadFacilities();
        LoadContacts();
        LoadHauling();
    }

    public void CloseStation()
    {
        this.currentStation = null;
        this.gameObject.SetActive(false);
    }

    private void InitializeAllPages()
    {
        pageStack = new PageStack();    // Initialize the page stack for this UI element
        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;   // Grab the overarching root visual element

        // Grab all the pages and store them as visual elements
        homeVisualElement = rootVisualElement.Q<VisualElement>("station-home");
        haulingVisualElement = rootVisualElement.Q<VisualElement>("station-hauling");
        contactsVisualElement = rootVisualElement.Q<VisualElement>("station-contacts");

        // Only display "home"
        haulingVisualElement.style.display = DisplayStyle.None;
        contactsVisualElement.style.display = DisplayStyle.None;

        haulingVisualElement.style.opacity = new StyleFloat(0.0);
        contactsVisualElement.style.opacity = new StyleFloat(0.0);

        pageStack.Push(homeVisualElement);
        nextPage = StationPage.HOME;

        backButton = rootVisualElement.Q<VisualElement>("back-button");
        exitButton = rootVisualElement.Q<VisualElement>("exit-button");
        
        rootVisualElement.Q<VisualElement>("button-contacts").RegisterCallback<ClickEvent>(ev => {
            nextPage = StationPage.CONTACT;
            homeVisualElement.style.opacity = new StyleFloat(0.0);
        });

        rootVisualElement.Q<VisualElement>("button-hauling").RegisterCallback<ClickEvent>(ev => {
            nextPage = StationPage.HAULING;
            homeVisualElement.style.opacity = new StyleFloat(0.0);
        });

        contactsVisualElement.Q<VisualElement>("contact-content-overlay").style.opacity = 0.0f;
        contactsVisualElement.Q<VisualElement>("contact-header-content").style.opacity = 0.0f;

        haulingVisualElement.Q<VisualElement>("hauling-header-content").style.display = DisplayStyle.None;
        haulingVisualElement.Q<VisualElement>("no-hauling-content").style.display = DisplayStyle.Flex;
        haulingVisualElement.Q<VisualElement>("cargo-destination").style.display = DisplayStyle.None;
        haulingVisualElement.Q<VisualElement>("cargo-item-content").style.display = DisplayStyle.None;

        haulingVisualElement.Q<VisualElement>("hauling-header-content").style.opacity = 0.0f;
        haulingVisualElement.Q<VisualElement>("no-hauling-content").style.opacity = 1.0f;
        haulingVisualElement.Q<VisualElement>("cargo-destination").style.opacity = 0.0f;
        haulingVisualElement.Q<VisualElement>("cargo-item-content").style.opacity = 0.0f;

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

                if (element.style.opacity == 0.0f)
                {
                    if (currHaulingContract != null)
                        haulingVisualElement.Q<VisualElement>("cargo-destination").style.opacity = 1.0f;
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
            CloseStation();
        });

        VisualElement convoChoices = contactsVisualElement.Q<VisualElement>("convo-choices");

        for (int i = 0; i < 3; i++)
        {
            convoChoices.ElementAt(i).style.opacity = 0.0f;
        }
        
    }

    private void LoadHauling()
    {
        VisualElement haulingContainer = haulingVisualElement.Q<VisualElement>("hauling-left");

        for (int i = 0; i < currentStation.contracts.Count; i++)
        {
            VisualElement haulingItem = haulingContainer.ElementAt(i);
            haulingItem.Q<Label>("hauling-jump-distance").text = Random.Range(5, 12).ToString() + " jumps away";
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
        }
        else
        {
            haulingVisualElement.Q<VisualElement>("hauling-header-content").style.opacity = 0.0f;
            haulingVisualElement.Q<VisualElement>("cargo-item-content").style.opacity = 0.0f;
            haulingVisualElement.Q<VisualElement>("cargo-destination").style.opacity = 0.0f;
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
