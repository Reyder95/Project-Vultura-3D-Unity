using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation
{
    private string prompt;
    private List<BaseResponse> responses;

    public Conversation(string prompt, List<BaseResponse> responses)
    {
        this.prompt = prompt;
        this.responses = responses;
    }

    public string Prompt
    {
        get
        {
            return this.prompt;
        }
    }

    public List<BaseResponse> Responses
    {
        get
        {
            return this.responses;
        }
    }
}
