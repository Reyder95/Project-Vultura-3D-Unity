using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalConvoHolder
{
    public static Dictionary<VulturaInstance.ContactType, List<Conversation>> conversationList = new Dictionary<VulturaInstance.ContactType, List<Conversation>>();

    public static void AddConversations(VulturaInstance.ContactType type, List<Conversation> conversations)
    {
        conversationList.Add(type, conversations);
    }

    // public static void LoadStationHeadConversations()
    // {
    //     List<Responses> responses1 = new List<Responses>();
    //     responses1.Add(ResponseFactory.CreateBasicResponse("I'm actually wondering if there's anyway I can be of help.", false));
    //     responses1.Add(ResponseFactory.CreateBasicResponse("Not at all, I'm sorry for bothering you", true));

    //     Conversation convo1 = new Conversation("Greetings, commander. I'd be careful out there, never know what kinds of baddies will wind up around the corner.\n\nAnything we can help you with?", responses1);
    // }
}
