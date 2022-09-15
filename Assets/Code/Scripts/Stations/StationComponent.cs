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

    public GameObject guiHome;          // The home GUI layer

    public GameObject GUIObject;

    public MiningStationUI GUI;

    // All contact page information
    public GameObject contactPrefab;

    public BaseStation station;    // The station associated with this prefab

    void Start()
    {
        GUI = GUIObject.GetComponent<MiningStationUI>();
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
    }

    // When a player presses the enter station button, this function gets called
    public void EnterStation()
    {
        if (canEnter)
        {
            GUI.OpenUI(station);
        }
    }

    // When the exit button is pressed
    public void ExitStation()
    {
    }

    // Loops and displays all contacts on the contacts screen
    public void DisplayContacts()
    {
    }

    // When a contact is pressed, we send that contact back
    public void ContactPress(Contact contact)
    {
        Debug.Log("Station name is " + station.SelectableName);
    }

    // When the contact button is pressed, go to the contacts page
    public void GoContacts()
    {
    }

    // If any "back" button is pressed, this will be called. 
    public void BackToHome()
    {
    }
    
}
