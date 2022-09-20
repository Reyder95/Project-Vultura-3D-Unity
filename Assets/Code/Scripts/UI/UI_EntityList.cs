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

    public struct DataStruct {
        public int entityIndex;
        public float distance;
    }

    public DataStruct[] entities;

    void Awake()
    {
        fleetListener = new UnityAction(SortAndPopulate);
        deselectedListener = new UnityAction(Deselected);
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
    }

    private void OnEnable()
    {
        EventManager.StartListening("Fleet Added", fleetListener);
        EventManager.StartListening("Deselect Ship", deselectedListener);
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
            } catch (ArgumentOutOfRangeException)
            {
            }
            

            e.RegisterCallback<ClickEvent>(ev => {
                try
                {
                    if (entities[i].entityIndex < VulturaInstance.fleetSelectables.Count)
                    {
                        if (Input.GetKey("left ctrl"))
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[entities[i].entityIndex], true, false);
                        }
                        else if (Input.GetKey("left alt"))
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[entities[i].entityIndex], false, true);
                        }
                        else
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.fleetSelectables[entities[i].entityIndex]);
                        }
                    }
                } catch (ArgumentOutOfRangeException)
                {
                }
            });
            
            try
            {
                if (entities[i].entityIndex < VulturaInstance.fleetSelectables.Count)
                {
                    if (VulturaInstance.fleetSelectables[entities[i].entityIndex].MainSelected)
                    {
                        e.EnableInClassList("normal", false);
                        e.EnableInClassList("main-selected-element", true);
                        e.EnableInClassList("selected-element", false);
                    }
                    else if (VulturaInstance.fleetSelectables[entities[i].entityIndex].Selected)
                    {
                        e.EnableInClassList("normal", false);
                        e.EnableInClassList("main-selected-element", false);
                        e.EnableInClassList("selected-element", true);
                    }
                    else
                    {
                        e.EnableInClassList("normal", true);
                        e.EnableInClassList("main-selected-element", false);
                        e.EnableInClassList("selected-element", false);
                    }
                }
            } catch (ArgumentOutOfRangeException)
            {
            }
            

            
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
