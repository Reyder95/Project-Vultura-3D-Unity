using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIScreenStack 
{
    private List<VisualElement> screenStack = new List<VisualElement>();

    public void Push(VisualElement uiScreen)
    {
        if (!screenStack.Contains(uiScreen))
        {
            screenStack.Add(uiScreen);
            uiScreen.BringToFront();
        }
    }

    public void RemoveElement(VisualElement uiScreen)
    {
        screenStack.Remove(uiScreen);
    }

    public void Access(VisualElement uiScreen)
    {
        RemoveElement(uiScreen);
        Push(uiScreen);
    }

    public List<VisualElement> ScreenStack 
    {
        get
        {
            return this.screenStack;
        }
    }
    
}