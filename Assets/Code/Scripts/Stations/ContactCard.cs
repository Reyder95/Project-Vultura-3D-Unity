using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script for the contact card prefab.
public class ContactCard : MonoBehaviour
{
    // The contact associated with this contact card
    public Contact contact;

    // All of the different GUI elements for this contact card
    public TMP_Text contactName;
    public Image contactPortrait;
    public TMP_Text contactType;
    public TMP_Text contactFaction;
    public Image contactFactionIcon;

    public StationComponent stationComponent;   // The station this contact card is within

    // Set the contact GUI text
    public void ContactText(Contact contact)
    {
        contactName.text = contact.Name;
        contactType.text = VulturaInstance.enumStringParser(contact.Type.ToString());
        contactFaction.text = contact.Faction;

        this.contact = contact;
    }

    // When button clicked, send the contact up to the station for further use.
    public void SendContactUp()
    {
        stationComponent.ContactPress(contact);
    }
}
