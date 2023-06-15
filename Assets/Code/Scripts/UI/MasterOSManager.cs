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
    public bool shipScreenOpen = false;

    public bool isDragging = false;

    public VisualElement tempTooltip;

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

    public void InitializeUI()
    {
        overElement = true;

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

            if (rootAsset.Q<VisualElement>("screen-background") != null)
            {
                rootAsset.style.display = DisplayStyle.None;
                
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
            }

            rootVisualElement.Add(rootAsset);

            visualDict.Add(uiStruct.windowName, rootAsset);
            screenObjects.Add(uiStruct.windowName, UIObjectFactory.InvokeFactory(uiStruct.windowName, new UIData(uiStruct.windowName, rootAsset)));

            if (screenObjects[uiStruct.windowName] != null)
            {
                screenObjects[uiStruct.windowName].Awake();
                screenObjects[uiStruct.windowName].SetLoadableAssets(uiStruct.assets);
                screenObjects[uiStruct.windowName].InitializeScreen();
            }


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
                        VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Swap((elementOver.userData as DragData).Index, (currentDraggedElement.userData as DragData).Index);
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

        OnEnable();
    }

    public void DraggerEvents(VisualElement item)
    {
        item.RegisterCallback<PointerDownEvent>(DragPointerDownEvent);
    }

    private void DragPointerDownEvent(PointerDownEvent ev)
    {
        VisualElement eventTarget = (ev.currentTarget as VisualElement);
        MasterOSManager.Instance.currentDraggedElement = eventTarget;
        MasterOSManager.Instance.currentDraggedElement.Q<VisualElement>("inventory-item").visible = false;

        Debug.Log(MasterOSManager.Instance.visualDragger.Q<Label>("item-count"));

        MasterOSManager.Instance.visualDragger.Q<Label>("item-count").text = eventTarget.Q<Label>("item-count").text;
        MasterOSManager.Instance.visualDragger.Q<Label>("item-name").text = eventTarget.Q<Label>("item-name").text;

        MasterOSManager.Instance.visualDragger.pickingMode = PickingMode.Ignore;
        MasterOSManager.Instance.visualDragger.Q<VisualElement>("inventory-item").pickingMode = PickingMode.Ignore;              
        MasterOSManager.Instance.visualDragger.style.position = Position.Absolute;
        MasterOSManager.Instance.visualDragger.style.visibility = Visibility.Visible;
        MasterOSManager.Instance.visualDragger.style.height = eventTarget.resolvedStyle.height;
        MasterOSManager.Instance.visualDragger.style.width = eventTarget.resolvedStyle.width;
        MasterOSManager.Instance.visualDragger.style.top = ev.position.y - eventTarget.resolvedStyle.height / 2;
        MasterOSManager.Instance.visualDragger.style.left = ev.position.x - eventTarget.resolvedStyle.width / 2;
        MasterOSManager.Instance.isDragging = true;

        if (tempTooltip != null)
        {
            try 
            {
                MasterOSManager.Instance.rootVisualElement.Remove(tempTooltip);
                tempTooltip = null;
            } catch (System.ArgumentException ex)
            {

            }

        }
    }

    public void OnEnable()
    {
        foreach (var item in screenObjects)
        {
            if (item.Value != null)
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
            if (item.Value != null)
                item.Value.Update();
        }
    }
}
