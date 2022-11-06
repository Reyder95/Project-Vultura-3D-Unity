using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contact
{
    private string name;
    private string faction;
    private VulturaInstance.ContactType type;

    public Contact(string name, string faction, VulturaInstance.ContactType type)
    {
        this.name = name;
        this.faction = faction;
        this.type = type;
    }

    public void LoadConversation()
    {
        // TODO
    }

    public string Name {
        get
        {
            return this.name;
        }
    }

    public string Faction {
        get
        {
            return this.faction;
        }
    }

    public VulturaInstance.ContactType Type
    {
        get
        {
            return this.type;
        }
    }
}
