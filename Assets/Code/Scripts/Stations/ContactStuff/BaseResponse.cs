using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseResponse
{
    private string prompt;
    private VulturaInstance.ResponseType type;

    public BaseResponse(string prompt, VulturaInstance.ResponseType type)
    {
        this.prompt = prompt;
        this.type = type;
    }   

    private string Prompt
    {
        get
        {
            return this.prompt;
        }
    }
}
