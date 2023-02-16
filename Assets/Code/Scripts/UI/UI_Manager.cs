using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UI_Manager
{
    public static List<UIDocument> mainUIElements = new List<UIDocument>();

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

    public static bool IsPointerOverUI (Vector2 screenPos)
    {
        Vector2 pointerUiPos = new Vector2(screenPos.x, Screen.height - screenPos.y);
        
        foreach (UIDocument uiDoc in mainUIElements)
        {
            List<VisualElement> picked = new List<VisualElement>();
            uiDoc.rootVisualElement.panel.PickAll(pointerUiPos, picked);

            foreach (var ve in picked)
            {
                Color32 bcol = ve.resolvedStyle.backgroundColor;

                if (bcol.a != 0 && ve.enabledInHierarchy)
                {
                    return true;
                    Debug.Log("Returning True!");
                }
            }
        }

        return false;
        Debug.Log("Returning False!");
    }
}
