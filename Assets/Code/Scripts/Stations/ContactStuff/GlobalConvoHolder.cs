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

    public static void LoadStationHeadConversations()
    {
        // Conversation 1
        List<BaseResponse> responses2 = new List<BaseResponse>();
        responses2.Add(ResponseFactory.CreateBasicResponse("Of course, have a good day.", true));
        Conversation convo2 = new Conversation("Not at the moment, commander. But we will let you know if there is anything we need help with.", responses2);

        List<BaseResponse> responses1 = new List<BaseResponse>();
        responses1.Add(ResponseFactory.CreateBasicResponse("I'm actually wondering if there's anyway I can be of help.", false, convo2));
        responses1.Add(ResponseFactory.CreateBasicResponse("Not at all, I'm sorry for bothering you", true));

        Conversation convo1 = new Conversation("Greetings, commander. I'd be careful out there, ya never know what kinds of baddies will wind up around the corner.\n\nAnything we can help you with?", responses1);
        
        List<Conversation> headConvos = new List<Conversation>();
        headConvos.Add(convo1);

        AddConversations(VulturaInstance.ContactType.Station_Head, headConvos);
    }
}
