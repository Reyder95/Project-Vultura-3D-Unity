using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Class to handle various UI elements (primarily the main UI elements)
public static class UI_Manager
{
    // The main UI elements of the game
    public static List<UIDocument> mainUIElements = new List<UIDocument>();

    // Load the main UI elements via the tag "MainUI"
    public static void LoadUIElements()
    {
        List<GameObject> UIObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("MainUI"));
        
        foreach (GameObject uiObject in UIObjects)
        {
            if (uiObject.TryGetComponent<UIDocument>(out UIDocument uiDoc))
            {
                mainUIElements.Add(uiDoc);
            }
        }
    }

    // Checks if the pointer is over a UI element.
    public static bool IsPointerOverUI (Vector2 screenPos)
    {
        Debug.Log(Screen.height - screenPos.y);
        Vector2 pointerUiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
        
        foreach (UIDocument uiDoc in mainUIElements)
        {
            Vector2 newPointerPos = RuntimePanelUtils.ScreenToPanel(uiDoc.rootVisualElement.panel, pointerUiPos);
            List<VisualElement> picked = new List<VisualElement>();
            uiDoc.rootVisualElement.panel.PickAll(newPointerPos, picked);

            foreach (var ve in picked)
            {
                Color32 bcol = ve.resolvedStyle.backgroundColor;

                if (bcol.a != 0 && ve.enabledInHierarchy)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
