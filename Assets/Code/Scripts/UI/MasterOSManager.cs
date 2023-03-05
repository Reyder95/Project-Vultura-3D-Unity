using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct UIStruct {
    public string windowName;
    public VisualTreeAsset asset;
}
public class MasterOSManager : MonoBehaviour
{
    public static MasterOSManager Instance { get; private set; }
    public UIStruct[] uiAssets;
    public VisualElement rootVisualElement;

    public Dictionary<string, VisualElement> visualDict = new Dictionary<string, VisualElement>();

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

        rootVisualElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("main-content");

        foreach (UIStruct uiStruct in uiAssets)
        {
            VisualElement rootAsset = uiStruct.asset.Instantiate();
            rootAsset.style.width = Length.Percent(100);
            rootAsset.style.height = Length.Percent(100);
            rootAsset.style.position = Position.Absolute;

            if (rootAsset.Q<VisualElement>("template-screen-nofaction") != null)
                rootAsset.Q<VisualElement>("template-screen-nofaction").name = "template-screen";
                
            rootAsset.name = uiStruct.windowName;
            rootAsset.style.display = DisplayStyle.None;
            rootVisualElement.Add(rootAsset);
            visualDict.Add(uiStruct.windowName, rootAsset);

            EventManager.TriggerEvent(uiStruct.windowName + " UI Event");
            GetComponent<UIWindowMovement>().InitializeMovementCallbacks(rootAsset);
        }
    }
}
