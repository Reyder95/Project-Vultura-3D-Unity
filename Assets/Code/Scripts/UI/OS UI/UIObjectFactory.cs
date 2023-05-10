using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIData {
    public VisualElement screen;
    public string windowName;

    public UIData(string windowName, VisualElement screen)
    {
        this.windowName = windowName;
        this.screen = screen;
    }
}

public static class UIObjectFactory 
{
    public static Dictionary<string, Func<UIData, BaseOS>> baseOSMap;

    public static BaseOS InvokeFactory(string key, UIData uiData)
    {
        Func<UIData, BaseOS> func = baseOSMap[key];
        return func(uiData);
    }

    public static void InitializeFactory()
    {
        baseOSMap = new Dictionary<string, Func<UIData, BaseOS>>();
        baseOSMap.Add("station", (data) => new StationUI(data.windowName, data.screen));
        baseOSMap.Add("inventory", (data) => new InventoryOS(data.windowName, data.screen));
    }
}