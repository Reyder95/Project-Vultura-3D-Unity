using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contact
{
    private string name;
    private string faction;
    private VulturaInstance.ContactType type;
    private Conversation conversation;

    public Contact(string name, string faction, VulturaInstance.ContactType type)
    {
        this.name = name;
        this.faction = faction;
        this.type = type;
    }

    public void LoadConversation()
    {
        Debug.Log("Loaded conversation");
        if (GlobalConvoHolder.conversationList.ContainsKey(this.type))
        {
            Debug.Log("Inside conversation");
            List<Conversation> convos = GlobalConvoHolder.conversationList[this.type];

            conversation = convos[Random.Range(0, convos.Count - 1)];
        }
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

    public Conversation Conversation
    {
        get
        {
            return this.conversation;
        }
    }
}
