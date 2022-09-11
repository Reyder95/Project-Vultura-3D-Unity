using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// A component that gets placed, by default, on each station prefab in the world. 
// The primary idea is for us to associate a station to a prefab. Anything necessary to handle physical station stuff will be handled here.
public class StationComponent : MonoBehaviour
{
    private bool canEnter = false;  // Determines, based on trigger, whether a player can enter this station or not.

    public TMP_Text stationEnterText;   // Enter text at the top of the screen when player gets close to station

    public Canvas stationGUI;           // The entire station GUI

    public GameObject guiHome;          // The home GUI layer

    // All contact page information
    public GameObject contactPrefab;
    public GameObject guiContacts;
    public GameObject contactsList;

    public BaseStation station;    // The station associated with this prefab

    void Awake()
    {
        stationGUI.enabled = false; // Immediately start with all station GUIs as disabled
    }

    // Sets the station and initializes its components. The idea is you should 
    // *only* initialize a station upon world load, or upon creation of a new station. 
    // Thus when a station is set, we immediately initialize it
    public void SetStation(BaseStation station)
    {
        this.station = station;

        // Initializes the station then displays the contacts in the contacts page
        station.InitializeStation();
        DisplayContacts();
    }

    // When a player enters the station proximity
    public void StationEnter()
    {
        canEnter = true;
        stationEnterText.enabled = true;
    }

    // When a player exits the station proximity
    public void StationExit()
    {
        canEnter = false;
        stationEnterText.enabled = false;
        stationGUI.enabled = false;
    }

    // When a player presses the enter station button, this function gets called
    public void EnterStation()
    {
        if (canEnter)
        {
            guiHome.SetActive(true);
            guiContacts.SetActive(false);
            stationGUI.enabled = true;
        }
    }

    // When the exit button is pressed
    public void ExitStation()
    {
        stationGUI.enabled = false;
    }

    // Loops and displays all contacts on the contacts screen
    public void DisplayContacts()
    {
        GameObject stationHeadUI = Instantiate(contactPrefab, contactsList.transform.GetChild(0));
        stationHeadUI.GetComponent<ContactCard>().ContactText(station.stationHead);
        stationHeadUI.GetComponent<ContactCard>().stationComponent = this;

        for (int i = 0; i < station.contacts.Count; i++)
        {
            GameObject stationContact = Instantiate(contactPrefab, contactsList.transform.GetChild(i + 1));
            stationContact.GetComponent<ContactCard>().ContactText(station.contacts[i]);
            stationContact.GetComponent<ContactCard>().stationComponent = this;
        }
    }

    // When a contact is pressed, we send that contact back
    public void ContactPress(Contact contact)
    {
        Debug.Log("Station name is " + station.SelectableName);
    }

    // When the contact button is pressed, go to the contacts page
    public void GoContacts()
    {
        guiHome.SetActive(false);
        guiContacts.SetActive(true);
    }

    // If any "back" button is pressed, this will be called. 
    public void BackToHome()
    {
        guiContacts.SetActive(false);
        guiHome.SetActive(true);
    }
    
}
