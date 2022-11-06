using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderResponse : BaseResponse
{
    // TODO: Add commander as a type of class

    public CommanderResponse(string prompt) : base(prompt, VulturaInstance.ResponseType.Commander)
    {
        
    }

}
