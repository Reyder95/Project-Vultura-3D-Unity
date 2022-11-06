using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResponseFactory
{
    public static BasicResponse CreateBasicResponse(string prompt, bool goBack, Conversation convo = null)
    {
        return new BasicResponse(prompt, convo, goBack);
    }

    public static CommanderResponse CreateCommanderResponse(string prompt)
    {
        return new CommanderResponse(prompt);
    }

    public static ShopResponse CreateShopResponse(string prompt, Market market)
    {
        return new ShopResponse(prompt, market);
    }
}
