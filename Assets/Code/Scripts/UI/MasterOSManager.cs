using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct UIAsset {
    public string assetKey;
    public VisualTreeAsset assetValue;
}

[System.Serializable]
public struct UIStruct {
    public string windowName;
    public VisualTreeAsset asset;
    public UIAsset[] assets;
}
public class MasterOSManager : MonoBehaviour
{
    public static MasterOSManager Instance { get; private set; }

    public UIStruct[] uiAssets;
    public VisualElement rootVisualElement;
    public VisualElement currentDraggedElement = null;
    public VisualElement visualDragger = null;
    public VisualElement elementOver = null;
    public bool overElement = true;

    public Dictionary<string, VisualElement> visualDict = new Dictionary<string, VisualElement>();

    Dictionary<string, BaseOS> screenObjects = new Dictionary<string, BaseOS>();

    public bool inventoryOpen = false;

    public bool isDragging = false;

    void Awake()
    {
        overElement = true;
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

        UIObjectFactory.InitializeFactory();

        rootVisualElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("main-content");
        visualDragger = rootVisualElement.Q<VisualElement>("ghost-item");

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

            rootAsset.Q<VisualElement>("screen-background").RegisterCallback<PointerEnterEvent>(ev => {
                if (isDragging)
                {
                    UIScreenManager.Instance.SetFocusedScreen(rootAsset);
                    overElement = true;
                }
            });

            rootAsset.Q<VisualElement>("screen-background").RegisterCallback<PointerLeaveEvent>(ev => {
                if (isDragging)
                {
                    Debug.Log("AAAHHHHH");
                    overElement = false;
                }
            });

            visualDict.Add(uiStruct.windowName, rootAsset);
            screenObjects.Add(uiStruct.windowName, UIObjectFactory.InvokeFactory(uiStruct.windowName, new UIData(uiStruct.windowName, rootAsset)));
            screenObjects[uiStruct.windowName].Awake();
            screenObjects[uiStruct.windowName].SetLoadableAssets(uiStruct.assets);
            screenObjects[uiStruct.windowName].InitializeScreen();

            EventManager.TriggerEvent(uiStruct.windowName + " UI Event");
            //GetComponent<UIWindowMovement>().InitializeMovementCallbacks(rootAsset);
        }

        rootVisualElement.RegisterCallback<PointerMoveEvent>(ev => {
            if (!isDragging)
                return;
                
            visualDragger.style.top = ev.position.y - visualDragger.layout.height / 2;
            visualDragger.style.left = ev.position.x - visualDragger.layout.width / 2;
        });

        rootVisualElement.RegisterCallback<PointerUpEvent>(ev => {

            if (isDragging)
            {
                if (!overElement)
                {
                    
                }

                isDragging = false;

                if (currentDraggedElement != null)
                    currentDraggedElement.Q<VisualElement>("inventory-item").visible = true;

                visualDragger.style.visibility = Visibility.Hidden;

                //VisualElement overlappingElement = FindOverlappingItem();

                if (elementOver != null)
                {
                    if (elementOver.userData != currentDraggedElement.userData)
                    {
                        VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Swap((int)elementOver.userData, (int)currentDraggedElement.userData);
                        EventManager.TriggerEvent("inventory UI Refresh");
                    }
                }
                else
                {
                    if (currentDraggedElement != null)
                        currentDraggedElement.Q<VisualElement>("inventory-item").visible = true;
                }
            }
        });
    }

    void OnEnable()
    {
        foreach (var item in screenObjects)
        {
            item.Value.OnEnable();
        }
    }

    void OnDisable()
    {
        foreach (var item in screenObjects)
        {
            item.Value.OnDisable();
        }
    }

    void Update()
    {
        if (visualDragger != null)
        {
            visualDragger.BringToFront();
        }

        foreach (var item in screenObjects)
        {
            item.Value.Update();
        }
    }
}
