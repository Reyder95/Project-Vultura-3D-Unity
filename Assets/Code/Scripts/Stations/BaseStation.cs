using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base station class. Each type of station will be derived from this one
public class BaseStation : BaseSelectable
{
    public List<Contact> contacts = new List<Contact>();
    public Contact stationHead;

    // Constructor
    public BaseStation(string faction, string selectableName, string type) : base(faction, selectableName, type)
    {
    }

    // Code that initializes the station and runs each function for initialization.
    public virtual void InitializeStation() 
    {
        int numContacts = Random.Range(2, 5);

        for (int i = 0; i < numContacts; i++)
        {
            contacts.Add(new Contact("Akane Mioka", "SomeFaction", (VulturaInstance.ContactType)Random.Range(1, 4)));
        }

        stationHead = new Contact("Akane Mioka", "SomeFaction", VulturaInstance.ContactType.Station_Head);

        foreach (Contact contact in contacts)
        {
            Debug.Log(VulturaInstance.enumStringParser(contact.Type.ToString()));
        }

        Debug.Log("Station Initialized");
    }
}
