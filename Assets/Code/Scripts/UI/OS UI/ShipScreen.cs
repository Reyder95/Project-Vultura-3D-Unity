using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

class ShipScreen : BaseOS
{
    private VisualElement shipScreenElement;
    private UnityAction openListener;
    private UnityAction initListener;
    private UnityAction equipListener;

    List<VisualElement> moduleElements = new List<VisualElement>();

    public ShipScreen(string windowName, VisualElement screen) : base(windowName, screen) {}

    public override void Awake()
    {
        openListener = new UnityAction(OpenScreen);
        initListener = new UnityAction(InitializeScreen);
        equipListener = new UnityAction(DisplayTurretMounts);
    }

    public override void Update()
    {
        if (screen != null)
        {
            if (UIScreenManager.Instance.focusedScreen == screen)
            {
                screen.Q<VisualElement>("screen-background").style.opacity = 1.0f;
            }
            else
            {
                screen.Q<VisualElement>("screen-background").style.opacity = 0.2f;
            }
        }
    }

    public override void OnEnable()
    {
        Debug.Log("ENABLED!!");
        EventManager.StartListening("ship-screen UI Open", openListener);
        EventManager.StartListening("ship-screen UI Event", initListener);
        EventManager.StartListening("equipped", equipListener);
    }

    public override void OnDisable()
    {
        EventManager.StopListening("ship-screen UI Open", openListener);
        EventManager.StopListening("ship-screen UI Event", initListener);
        EventManager.StopListening("equipped", equipListener);
    }

    public override void InitializeScreen()
    {
        windowName = "ship-screen";

        base.InitializeScreen();

        foreach (VisualElement e in screen.Q<VisualElement>("module-list").Children())
        {
            moduleElements.Add(e);
        }
        

    }

    private void EquipEvent(PointerUpEvent ev)
    {
        Inventory draggedInventory = (MasterOSManager.Instance.currentDraggedElement.userData as DragData).InvFrom;
        int index = (MasterOSManager.Instance.currentDraggedElement.userData as DragData).Index;
        InstantiatedShip currShip = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip;

        if (draggedInventory.itemList[index].item is ActiveModule)
        {
            GameObject turretMount = (ev.currentTarget as VisualElement).userData as GameObject;

            currShip.EquipMount((int)((ev.currentTarget as VisualElement).userData), draggedInventory.itemList[index].item as ActiveModule);
        }
    }

    private void DisplayTurretMounts()
    {
        InstantiatedShip playerShip = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip;

        int numElements = moduleElements.Count;

        for (int i = 0; i < moduleElements.Count; i++)
        {
            if (i < playerShip.turretMounts.Count)
            {
                moduleElements[i].style.display = DisplayStyle.Flex;
                moduleElements[i].userData = i;

                if (playerShip.turretMounts[i].GetComponent<MountComponent>().turret != null)
                    moduleElements[i].Q<VisualElement>("module-icon").style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 255));
                else
                    moduleElements[i].Q<VisualElement>("module-icon").style.backgroundColor = new StyleColor(new Color32(0, 0, 0, 255));

                moduleElements[i].RegisterCallback<PointerUpEvent>(EquipEvent);
            }
            else
            {
                moduleElements[i].style.display = DisplayStyle.None;
            }
        }
    }

    public override void OpenScreen()
    {
        base.OpenScreen();

        MasterOSManager.Instance.shipScreenOpen = !MasterOSManager.Instance.shipScreenOpen;

        DisplayTurretMounts();

        if (MasterOSManager.Instance.shipScreenOpen)
            screen.style.display = DisplayStyle.Flex;
        else
        {
            screen.style.display = DisplayStyle.None;
        }
    }
}