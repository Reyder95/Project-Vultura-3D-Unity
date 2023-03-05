using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIScreenManager : MonoBehaviour
{
    public static UIScreenManager Instance { get; private set; }

    public static UIScreenStack screenStack = new UIScreenStack();

    public VisualElement focusedScreen = null;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        // Singleton handler
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Update() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandlePointerOver();
        }
    }

    public void AddScreen(VisualElement uiScreen)
    {
        screenStack.Access(uiScreen);
    }

    public void RemoveScreen(VisualElement uiScreen)
    {
        screenStack.RemoveElement(uiScreen);
    }

    public void SetFocusedScreen(VisualElement uiScreen)
    {
        focusedScreen = uiScreen;

        AddScreen(uiScreen);
    }

    public void UnfocusScreen()
    {   
        focusedScreen = null;
    }

    public void HandlePointerOver()
    {
        if (IsPointerOverUI(Input.mousePosition))
        {
            Debug.Log("Over!");
        }
        else
        {
            Debug.Log("Not over!");
        }
    }

    public bool IsPointerOverUI (Vector2 screenPos)
    {
        Vector2 pointerUiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
        
        for (int i = screenStack.ScreenStack.Count - 1; i >= 0; i--)
        {
            Vector2 newPointerPos = RuntimePanelUtils.ScreenToPanel(screenStack.ScreenStack[i].panel, pointerUiPos);
            List<VisualElement> picked = new List<VisualElement>();
            screenStack.ScreenStack[i].panel.PickAll(newPointerPos, picked);

            foreach (var ve in picked)
            {
                Color32 bcol = ve.resolvedStyle.backgroundColor;

                if (bcol.a != 0 && ve.enabledInHierarchy)
                {
                    //SetFocusedScreen(screenStack.ScreenStack[i]);
                    return true;
                }
            }
        }        

        UnfocusScreen();

        return false;
    }
}
