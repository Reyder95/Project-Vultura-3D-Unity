using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class SideScreen : BaseOS
{

    private UnityAction initListener;
    private UnityAction refreshListener;

    private ListView mainEntityList;
    private ListView subEntityList;

    public int elementHovered = -1;

    public StyleColor defaultColor = new StyleColor(new Color32(0, 0, 0, 0));
    public StyleColor mainSelected = new StyleColor(new Color32(43, 131, 245, 100));
    public StyleColor selected = new StyleColor(new Color32(89, 158, 247, 100));

    public SideScreen(string windowName, VisualElement screen) : base(windowName, screen) {}

    public override void Awake()
    {
        initListener = new UnityAction(InitializeScreen);
        refreshListener = new UnityAction(RefreshEvent);
    }

    public override void OnEnable()
    {
        EventManager.StartListening("side-screen UI Event", initListener);
        EventManager.StartListening("RefreshList", refreshListener);
    }

    public override void OnDisable()
    {
        EventManager.StopListening("side-screen UI Event", initListener);
        EventManager.StopListening("RefreshList", refreshListener);
    }

    public override void InitializeScreen()
    {
        windowName = "side-screen";

        mainEntityList = screen.Q<ListView>("main-list");
        subEntityList = screen.Q<ListView>("sub-list");

        Func<VisualElement> makeItem = () => {
            VisualElement entityItem = loadableAssets["entity-item"].Instantiate();

            entityItem.style.width = Length.Percent(100);

            return entityItem;
        };

        Func<VisualElement> makeItemSub = () => {
            VisualElement entityItem = loadableAssets["entity-item"].Instantiate();

            entityItem.style.width = Length.Percent(100);

            Debug.Log("HALLLLOOOO");

            return entityItem;
        };

        Action<VisualElement, int> bindItemMain = (e, i) => {
            e.Q<Label>("name").text = VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].SelectableName;
            e.Q<Label>("distance").text = EntitySorter.Instance.entities[i].distance.ToString("N2") + " km";
            e.Q<Label>("faction").text = VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].Faction;
            e.Q<Label>("type").text = VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].Type;

            try {
                if (VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].MainSelected)
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = mainSelected;
                    else
                        e.style.backgroundColor = mainSelected;
                }
                else if (VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].Selected)
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = selected;
                    else
                        e.style.backgroundColor = selected;
                } else
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = defaultColor;
                    else
                        e.style.backgroundColor = defaultColor;
                }
            } catch (ArgumentOutOfRangeException)
            {

            }

            e.RegisterCallback<PointerEnterEvent>(ev => {
                elementHovered = i;
                if (VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].MainSelected)
                {
                    e.style.backgroundColor = mainSelected;
                }
                else if (VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].Selected)
                {
                    e.style.backgroundColor = selected;
                } else
                {
                    e.style.backgroundColor = selected;
                }
            });

            e.RegisterCallback<PointerLeaveEvent>(ev => {
                elementHovered = -1;
                if (VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].MainSelected)
                {
                    e.style.backgroundColor = mainSelected;
                }
                else if (VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex].Selected)
                {
                    e.style.backgroundColor = selected;
                } else
                {
                    e.style.backgroundColor = defaultColor;
                }
            });

            e.RegisterCallback<PointerDownEvent>(ev => {
                try
                {
                    if (EntitySorter.Instance.entities[i].entityIndex < VulturaInstance.systemSelectables.Count)
                    {
                        if (Input.GetKey("left ctrl"))
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex], true, false);
                        }
                        else if (Input.GetKey("left alt"))
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex], false, true);
                        }
                        else
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.systemSelectables[EntitySorter.Instance.entities[i].entityIndex]);
                        }
                    }

                    RefreshEvent();
                } catch (ArgumentOutOfRangeException)
                {
                }
            });
        };

        Action<VisualElement, int> bindItemSub = (e, i) => {
            e.Q<Label>("name").text = VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].SelectableName;
            e.Q<Label>("distance").text = EntitySorter.Instance.subEntities[i].distance.ToString("N2") + " km";
            e.Q<Label>("faction").text = VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].Faction;
            e.Q<Label>("type").text = VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].Type;

            Debug.Log("HELLOOOOOO");

            try {
                if (VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].MainSelected)
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = mainSelected;
                    else
                        e.style.backgroundColor = mainSelected;
                }
                else if (VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].Selected)
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = selected;
                    else
                        e.style.backgroundColor = selected;
                } else
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = defaultColor;
                    else
                        e.style.backgroundColor = defaultColor;
                }
            } catch (ArgumentOutOfRangeException)
            {

            }

            e.RegisterCallback<PointerEnterEvent>(ev => {
                elementHovered = i;
                if (VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].MainSelected)
                {
                    e.style.backgroundColor = mainSelected;
                }
                else if (VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].Selected)
                {
                    e.style.backgroundColor = selected;
                } else
                {
                    e.style.backgroundColor = selected;
                }
            });

            e.RegisterCallback<PointerLeaveEvent>(ev => {
                elementHovered = -1;
                if (VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].MainSelected)
                {
                    e.style.backgroundColor = mainSelected;
                }
                else if (VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex].Selected)
                {
                    e.style.backgroundColor = selected;
                } else
                {
                    e.style.backgroundColor = defaultColor;
                }
            });

            e.RegisterCallback<PointerDownEvent>(ev => {
                try
                {
                    if (EntitySorter.Instance.subEntities[i].entityIndex < VulturaInstance.fleetSelectables.Count)
                    {
                        if (Input.GetKey("left ctrl"))
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex], true, false);
                        }
                        else if (Input.GetKey("left alt"))
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex], false, true);
                        }
                        else
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[EntitySorter.Instance.subEntities[i].entityIndex]);
                        }
                    }

                    RefreshEvent();
                } catch (ArgumentOutOfRangeException)
                {
                }
            });
        };



        mainEntityList.makeItem = makeItem;
        mainEntityList.bindItem = bindItemMain;
        mainEntityList.itemsSource = EntitySorter.Instance.entities;

        subEntityList.makeItem = makeItemSub;
        subEntityList.bindItem = bindItemSub;
        subEntityList.itemsSource = EntitySorter.Instance.subEntities;
    }

    private void RefreshEvent()
    {
        mainEntityList.itemsSource = EntitySorter.Instance.entities;
        subEntityList.itemsSource = EntitySorter.Instance.subEntities;
        mainEntityList.Rebuild();
        subEntityList.Rebuild();
    }
}
