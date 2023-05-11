using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class BaseOS
{
    public string windowName;
    public VisualElement screen;
    public Dictionary<string, VisualTreeAsset> loadableAssets = new Dictionary<string, VisualTreeAsset>();

    public BaseOS(string windowName, VisualElement screen)
    {
        this.windowName = windowName;
        this.screen = screen;
    }

    public virtual void Awake() {}
    public virtual void OnEnable() {}
    public virtual void OnDisable() {}
    public virtual void Update() {}

    public void SetLoadableAssets(UIAsset[] assets)
    {
        foreach (UIAsset asset in assets)
        {
            this.loadableAssets.Add(asset.assetKey, asset.assetValue);
        }
    }

    public virtual void InitializeScreen()
    {
        if (windowName != null)
        {
            screen = MasterOSManager.Instance.visualDict[windowName];

        }

    }

    public virtual void OpenScreen()
    {
        UIScreenManager.Instance.SetFocusedScreen(screen);
        MasterOSManager.Instance.GetComponent<UIWindowMovement>().InitializeMovementCallbacks(screen);

        screen.Q<VisualElement>("screen-background").RegisterCallback<PointerDownEvent>(HandleFocus);
        screen.Q<VisualElement>("screen-header").RegisterCallback<PointerDownEvent>(ev => { 
            MasterOSManager.Instance.GetComponent<UIWindowMovement>().SetDragging(null);
        });
    }

    public void HandleFocus(PointerDownEvent ev)
    {
        UIScreenManager.Instance.SetFocusedScreen(screen);
        MasterOSManager.Instance.GetComponent<UIWindowMovement>().InitializeMovementCallbacks(screen);
    }

    public virtual void CloseScreen()
    {
    }

    
}
