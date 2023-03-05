using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class BaseOS : MonoBehaviour
{
    public string windowName = null;
    public VisualElement screen = null;

    public virtual void InitializeScreen()
    {
        
        if (windowName != null)
            screen = MasterOSManager.Instance.visualDict[windowName];

    }

    public virtual void OpenScreen()
    {
        UIScreenManager.Instance.SetFocusedScreen(screen);
        screen.Q<VisualElement>("screen-background").RegisterCallback<PointerDownEvent>(ev => {
            UIScreenManager.Instance.SetFocusedScreen(screen);
        });
    }

    public virtual void CloseScreen()
    {
    }

    
}
