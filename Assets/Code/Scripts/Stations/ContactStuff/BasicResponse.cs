using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicResponse : BaseResponse
{
    private Conversation conversation;
    private bool goBack;

    public BasicResponse(string prompt, Conversation conversation, bool goBack) : base(prompt, VulturaInstance.ResponseType.Basic)
    {
        this.conversation = conversation;
        this.goBack = goBack;
    }

    public Conversation Conversation
    {
        get
        {
            return this.conversation;
        }
    }

    public bool GoBack
    {
        get
        {
            return this.goBack;
        }
    }
}
