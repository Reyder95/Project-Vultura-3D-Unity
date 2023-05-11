using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StationContact : OSUIHandler
{
    private VisualElement contactsVisualElement;        // Page visual element
    private VisualElement currContact = null;           // The current contact that we are utilizing
    private ConversationStack convoStack;               // The conversation stack when discussing 
    private VisualElement contactContentOverlay;        // The overlay for displaying
    private VisualElement contactHeaderContent;         // The header content for contacts
    private VisualElement contactNoSelectionContent;    // The area for no contact selection
    private VisualElement contactRight;                 // Right contact pane
    private VisualElement convoChoices;                 // Visual element containing the conversation choice buttons

    // Grab the information from the UXML, store them, and hide them.
    public override void SetTaggedReferences(VisualElement screen, StationUI station)
    {
        uiComponent = station;

        contactsVisualElement = screen.Q<VisualElement>("station-contacts");        
        
        contactContentOverlay = contactsVisualElement.Q<VisualElement>("contact-content-overlay");
        contactHeaderContent = contactsVisualElement.Q<VisualElement>("contact-header-content");
        contactNoSelectionContent = contactsVisualElement.Q<VisualElement>("contact-no-selection-content");
        contactRight = contactsVisualElement.Q<VisualElement>("contacts-right");
        convoChoices = contactsVisualElement.Q<VisualElement>("convo-choices");

        contactsVisualElement.style.display = DisplayStyle.None;

        contactsVisualElement.style.opacity = 0.0f;
        contactContentOverlay.style.opacity = 0.0f;
        contactHeaderContent.style.opacity = 0.0f;

        for (int i = 0; i < 3; i++)
        {
            convoChoices.ElementAt(i).style.opacity = 0.0f;
        }
        
    }

    // Set the callbacks for each element in the UXML
    public override void SetCallbacks()
    {
        contactContentOverlay.RegisterCallback<TransitionEndEvent>(contactContentEndTransition);
        contactNoSelectionContent.RegisterCallback<TransitionEndEvent>(contactNoSelectionEndTransition);
        contactHeaderContent.RegisterCallback<TransitionEndEvent>(contactHeaderEndTransition);
        contactsVisualElement.RegisterCallback<TransitionEndEvent>(contactPageTransitionEnd);
    }

    // Return the page element of this object
    public override VisualElement ReturnPage()
    {
        return contactsVisualElement;
    }

    //  Set the contact to null
    public void ClearContact()
    {
        this.currContact = null;
    }

    private void SetCurrentContact(VisualElement contact)
    {
        if (contact != null)
        {
            if (currContact != null)
                currContact.EnableInClassList("active-facility-button", false);
            currContact = contact;

            Contact myContact = (contact.userData as Contact);

            contactNoSelectionContent.style.opacity = 0.0f;
            contact.EnableInClassList("active-facility-button", true);
        }
        else
        {
            
            currContact.EnableInClassList("active-facility-button", false);
            currContact = null;

            contactHeaderContent.style.opacity = 0.0f;

            contactContentOverlay.style.opacity = 0.0f;

            for (int i = 0; i < 3; i++)
            {
                convoChoices.ElementAt(i).style.opacity = 0.0f;
            }
        }
    }

    // Load the contacts on the screen and their data
    public void LoadContacts()
    {
        VisualElement contactContainer = contactsVisualElement.Q<VisualElement>("contacts-left");

        VisualElement headItem = contactContainer.ElementAt(0);
        headItem.Q<Label>("contact-name").text = uiComponent.currentStation.stationHead.Name;
        headItem.Q<Label>("contact-role").text = VulturaInstance.enumStringParser(uiComponent.currentStation.stationHead.Type.ToString());
        headItem.userData = uiComponent.currentStation.stationHead;
        headItem.style.visibility = Visibility.Visible;

        headItem.RegisterCallback<ClickEvent>(ev => {
            if (currContact == (ev.currentTarget as VisualElement))
                SetCurrentContact(null);
            else
                SetCurrentContact((ev.currentTarget as VisualElement));
        });

        for (int i = 0; i < uiComponent.currentStation.contacts.Count; i++)
        {
            VisualElement contactItem = contactContainer.ElementAt(i+1);
            contactItem.Q<Label>("contact-name").text = uiComponent.currentStation.contacts[i].Name;
            contactItem.Q<Label>("contact-role").text = VulturaInstance.enumStringParser(uiComponent.currentStation.contacts[i].Type.ToString());
            contactItem.userData = uiComponent.currentStation.contacts[i];
            contactItem.style.visibility = Visibility.Visible;

            contactItem.RegisterCallback<ClickEvent>(ev => {
                if (currContact == (ev.currentTarget as VisualElement))
                    SetCurrentContact(null);
                else
                    SetCurrentContact((ev.currentTarget as VisualElement));
            });
        }
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
            contactContentOverlay.style.display = DisplayStyle.None;
            convoChoices.style.display = DisplayStyle.None;

            //contactRight.style.display = DisplayStyle.None;
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
                } 
            }
        }
    }

    // Handles the transition when the contact content ends a transition
    private void contactContentEndTransition(TransitionEndEvent ev)
    {
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
    }

    // Handles the transition when the contact no selection ends a transition
    private void contactNoSelectionEndTransition(TransitionEndEvent ev)
    {
        if (ev.stylePropertyNames.Contains("opacity"))
        {
            VisualElement element = (ev.currentTarget as VisualElement);
            
            if (element.style.opacity == 0.0f)
            {
                contactNoSelectionContent.style.display = DisplayStyle.None;
                contactHeaderContent.style.display = DisplayStyle.Flex;
                contactRight.Q<Label>("contact-name").text = (currContact.userData as Contact).Name;
                contactRight.Q<Label>("contact-role").text = VulturaInstance.enumStringParser((currContact.userData as Contact).Type.ToString());
                Debug.Log("Test!");
                contactHeaderContent.style.opacity = 1.0f;
            }
        }
    }

    // Handles the transition when the contact headers ends a transition
    private void contactHeaderEndTransition(TransitionEndEvent ev)
    {
        if (ev.stylePropertyNames.Contains("opacity"))
        {
            VisualElement element = (ev.currentTarget as VisualElement);
            
            if (element.style.opacity == 1.0f)
            {
                InitializeConversationStack();
            }
            else if (element.style.opacity == 0.0f)
            {
                contactNoSelectionContent.style.display = DisplayStyle.Flex;
                contactNoSelectionContent.style.opacity = 1.0f;
            }
        }
    }

    // Handles the transition when the contact page ends a transition
    private void contactPageTransitionEnd(TransitionEndEvent ev)
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

}