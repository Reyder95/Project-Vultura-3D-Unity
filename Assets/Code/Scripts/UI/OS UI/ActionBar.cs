using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class ActionBar : BaseOS
{
    private VisualElement actionBarElement;
    private List<VisualElement> elementBoxes = new List<VisualElement>();

    private UnityAction refreshListener;

    float activeOpacity = 1.0f;
    float inactiveOpacity = 0.2f;

    Color32 equippedColor = new Color32(255, 255, 255, 255);
    Color32 unequippedColor = new Color32(128, 128, 128, 255);
    public ActionBar(string windowName, VisualElement screen): base(windowName, screen) {}

    public override void Awake()
    {
        refreshListener = new UnityAction(RefreshBar);
    }

    public override void Update()
    {
        foreach (VisualElement e in elementBoxes)
        {
            if (e.userData != null)
            {
                if (Input.GetKeyDown(((int)e.userData).ToString()))
                {
                    if (VulturaInstance.selectorList.mainSelected != null)
                    {
                        if (VulturaInstance.selectorList.mainSelected.selectableObject.tag == "Asteroid")
                        {
                            InstantiatedShip playerShip = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip;

                            if (playerShip.turretMounts.Count != 0)
                                playerShip.turretMounts[(int)e.userData - 1].GetComponent<MountComponent>().UseTurret(VulturaInstance.selectorList.mainSelected);

                        }
                    }
                    else
                    {
                        InstantiatedShip playerShip = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip;
                            if (playerShip.turretMounts.Count != 0)
                                playerShip.turretMounts[(int)e.userData - 1].GetComponent<MountComponent>().StopTurret();
                    }
                }
            }

        }
    }

    public override void OnEnable()
    {
        EventManager.StartListening("equipped", refreshListener);
    }

    public override void OnDisable()
    {
        EventManager.StopListening("equipped", refreshListener);
    }

    public override void InitializeScreen()
    {
        windowName = "action-bar";

        InstantiatedShip playerShip = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip;

        actionBarElement = screen.Q<VisualElement>("action-bar-element");

        foreach (VisualElement e in actionBarElement.Children())
        {
            elementBoxes.Add(e);
        }

        for (int i = 0; i < elementBoxes.Count; i++)
        {
            if (i < playerShip.turretMounts.Count)
            {
                elementBoxes[i].style.backgroundColor = new StyleColor(unequippedColor);
                elementBoxes[i].userData = i+1;
                elementBoxes[i].style.opacity = activeOpacity;
            }
            else
            {
                elementBoxes[i].style.backgroundColor = new StyleColor(unequippedColor);
                elementBoxes[i].style.opacity = inactiveOpacity;
            }
        }

    }

    private void RefreshBar()
    {
        InstantiatedShip playerShip = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip;

        for (int i = 0; i < elementBoxes.Count; i++)
        {
            if (i < playerShip.turretMounts.Count)
            {
                if (playerShip.turretMounts[i].GetComponent<MountComponent>().turret != null)
                    elementBoxes[i].style.backgroundColor = new StyleColor(equippedColor);
                else
                    elementBoxes[i].style.backgroundColor = new StyleColor(unequippedColor);

                elementBoxes[i].style.opacity = activeOpacity;
            }
            else
            {
                elementBoxes[i].style.backgroundColor = new StyleColor(unequippedColor);
                elementBoxes[i].style.opacity = inactiveOpacity;
            }
        }
    }
}