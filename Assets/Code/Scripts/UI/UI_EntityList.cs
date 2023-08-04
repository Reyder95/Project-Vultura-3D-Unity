using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class UI_EntityList : MonoBehaviour
{
    private ListView entityListView;
    private Button entityButton;
    private VisualElement myVisualElement;
    public GameObject myDocument;
    public StyleSheet entityMainSelected;
    public StyleSheet entitySelected;
    public VisualTreeAsset listItem;
    public VisualElement rootVisualElement;

    private UnityAction fleetListener;
    private UnityAction deselectedListener;
    private UnityAction cycleListener;

    public int elementHovered = -1;

    public struct DataStruct {
        public int entityIndex;
        public float distance;
    }

    public DataStruct[] entities;

    void Awake()
    {
        fleetListener = new UnityAction(SortAndPopulate);
        deselectedListener = new UnityAction(Deselected);
        cycleListener = new UnityAction(RefreshEvent);
    }

    void Update()
    {
        
        try
        {
            if (!rootVisualElement.visible && entities.Length > 0)
                rootVisualElement.visible = true;
            else if (rootVisualElement.visible && entities.Length == 0)
                rootVisualElement.visible = false;
        } catch (NullReferenceException)
        {
        }
        
        
    }
    
    void OnDisable()
    {
        EventManager.StopListening("Fleet Added", fleetListener);
        EventManager.StopListening("Deselect Ship", deselectedListener);
        EventManager.StopListening("Cycle Ship", cycleListener);
    }

    private void OnEnable()
    {
        EventManager.StartListening("Fleet Added", fleetListener);
        EventManager.StartListening("Deselect Ship", deselectedListener);
        EventManager.StartListening("Cycle Ship", cycleListener);

        rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        entityListView = rootVisualElement.Q<ListView>("fleet-list");
        entityButton = rootVisualElement.Q<Button>("list-button");

        Func<VisualElement> makeItem = () => listItem.Instantiate();
        Action<VisualElement, int> bindItem = (e, i) => {

            try
            {
                var faction = e.Q<Label>("faction");
                faction.text = VulturaInstance.fleetSelectables[entities[i].entityIndex].Faction;

                var name = e.Q<Label>("name");
                name.text = VulturaInstance.fleetSelectables[entities[i].entityIndex].SelectableName;

                var type = e.Q<Label>("type");
                type.text = VulturaInstance.fleetSelectables[entities[i].entityIndex].Type;

                var distance = e.Q<Label>("distance");
                distance.text = entities[i].distance.ToString();

                if (VulturaInstance.fleetSelectables[entities[i].entityIndex].MainSelected)
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = new StyleColor(new Color32(214, 41, 41, 102));
                    else
                        e.style.backgroundColor = new StyleColor(new Color32(163, 33, 33, 102));
                }
                else if (VulturaInstance.fleetSelectables[entities[i].entityIndex].Selected)
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = new StyleColor(new Color32(144, 44, 201, 102));
                    else
                        e.style.backgroundColor = new StyleColor(new Color32(107, 32, 150, 102));
                } else
                {
                    if (i == elementHovered)
                        e.style.backgroundColor = new StyleColor(new Color32(227, 227, 227, 102));
                    else
                        e.style.backgroundColor = new StyleColor(new Color32(176, 176, 176, 102));
                }


            } catch (ArgumentOutOfRangeException)
            {
            }

        e.RegisterCallback<PointerEnterEvent>(ev => {
            elementHovered = i;
            if (VulturaInstance.fleetSelectables[entities[i].entityIndex].MainSelected)
            {
                e.style.backgroundColor = new StyleColor(new Color32(214, 41, 41, 102));
            }
            else if (VulturaInstance.fleetSelectables[entities[i].entityIndex].Selected)
            {
                e.style.backgroundColor = new StyleColor(new Color32(144, 44, 201, 102));
            } else
            {
                e.style.backgroundColor = new StyleColor(new Color32(227, 227, 227, 102));
            }
        });

        e.RegisterCallback<PointerLeaveEvent>(ev => {
            elementHovered = -1;
            if (VulturaInstance.fleetSelectables[entities[i].entityIndex].MainSelected)
            {
                e.style.backgroundColor = new StyleColor(new Color32(163, 33, 33, 102));
            }
            else if (VulturaInstance.fleetSelectables[entities[i].entityIndex].Selected)
            {
                e.style.backgroundColor = new StyleColor(new Color32(107, 32, 150, 102));
            } else
            {
                e.style.backgroundColor = new StyleColor(new Color32(176, 176, 176, 102));
            }
        });
            

            // e.RegisterCallback<PointerDownEvent>(ev => {
            //     try
            //     {
            //         if (entities[i].entityIndex < VulturaInstance.fleetSelectables.Count)
            //         {
            //             if (Input.GetKey("left ctrl"))
            //             {
            //                 VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[entities[i].entityIndex], true, false);
            //             }
            //             else if (Input.GetKey("left alt"))
            //             {
            //                 VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[entities[i].entityIndex], false, true);
            //             }
            //             else
            //             {
            //                 VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[entities[i].entityIndex]);
            //             }
            //         }
            //     } catch (ArgumentOutOfRangeException)
            //     {
            //     }
            // });
            

            
        };

        entityListView.makeItem = makeItem;
        entityListView.bindItem = bindItem;
        entityListView.itemsSource = entities;

        InvokeRepeating("SortAndPopulate", 0f, 2.0f);
    }

    private async void SortAndPopulate()
    {
        NativeList<DataStruct> newList = new NativeList<DataStruct>(Allocator.TempJob);

        for (int i = 0; i < VulturaInstance.fleetSelectables.Count; i++)
        {
            DataStruct newDistance = new DataStruct {
                entityIndex = i,
                distance = VulturaInstance.CalculateDistance(VulturaInstance.currentPlayer, VulturaInstance.fleetSelectables[i].selectableObject),
            };

            newList.Add(newDistance);
        }

        var result = await Task.Run(() => {
            newList.Sort(new EntitySort());

            return newList;
        });

        entities = newList.ToArray();
        newList.Dispose();
        Refresh();
    }

    private void Deselected()
    {
        SortAndPopulate();
        Refresh();
    }

    // A refresh that calls SortAndPopulate through an event (via tabbing)
    private void RefreshEvent()
    {
        SortAndPopulate();
    }

    private void Refresh()
    {
        
        entityListView.itemsSource = entities;
        entityListView.Rebuild();
        
    }

    public struct EntitySort : IComparer<DataStruct>
    {
        public int Compare(DataStruct a, DataStruct b)
        {
            if (a.distance - b.distance < 0f)
                return -1;

            return 1;
        }
    }
}
