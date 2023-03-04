using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIScreenManager : MonoBehaviour
{
    public static UIScreenManager Instance { get; private set; }

    public static List<UIDocument> UIScreens = new List<UIDocument>();

    public UIDocument focusedScreen = null;

    void Start()
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

    public void AddScreen(UIDocument uiScreen)
    {
        if (!UIScreens.Contains(uiScreen))
        {
            UIScreens.Add(uiScreen);
        }
    }

    public void RemoveScreen(UIDocument uiScreen)
    {
        UIScreens.Remove(uiScreen);
    }

    public void SetFocusedScreen(UIDocument uiScreen)
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
        
        foreach (UIDocument uiDoc in UIScreens)
        {
            Vector2 newPointerPos = RuntimePanelUtils.ScreenToPanel(uiDoc.rootVisualElement.panel, pointerUiPos);
            List<VisualElement> picked = new List<VisualElement>();
            uiDoc.rootVisualElement.panel.PickAll(newPointerPos, picked);

            foreach (var ve in picked)
            {
                Color32 bcol = ve.resolvedStyle.backgroundColor;

                if (bcol.a != 0 && ve.enabledInHierarchy)
                {
                    SetFocusedScreen(uiDoc);
                    return true;
                }
            }
        }        

        UnfocusScreen();

        return false;
    }
}
