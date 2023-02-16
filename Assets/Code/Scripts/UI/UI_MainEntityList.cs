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

public class UI_MainEntityList : MonoBehaviour
{
    private ListView entityListView;
    private Button entityButton;
    private VisualElement myVisualElement;
    public GameObject myDocument;
    public StyleSheet entityMainSelected;
    public StyleSheet entitySelected;
    public VisualTreeAsset listItem;
    public UnityAction selectionChanged;
    public UnityAction cycleList;

    public struct DataStruct {
        public int entityIndex;
        public float distance;
    }

    public DataStruct[] entities;

    void Awake()
    {
        selectionChanged = new UnityAction(SortAndPopulate);
        cycleList = new UnityAction(RefreshEvent);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Selection Changed", selectionChanged);
        EventManager.StopListening("Cycle Ship", cycleList);
    }

    private void OnEnable()
    {
        EventManager.StartListening("Selection Changed", selectionChanged);
        EventManager.StartListening("Cycle Ship", cycleList);
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        entityListView = rootVisualElement.Q<ListView>("entity-list");
        entityButton = rootVisualElement.Q<Button>("list-button");

        Func<VisualElement> makeItem = () => listItem.Instantiate();
        Action<VisualElement, int> bindItem = (e, i) => {

            try
            {
                var faction = e.Q<Label>("faction");
                faction.text = VulturaInstance.systemSelectables[entities[i].entityIndex].Faction;

                var name = e.Q<Label>("name");
                name.text = VulturaInstance.systemSelectables[entities[i].entityIndex].SelectableName;

                var type = e.Q<Label>("type");
                type.text = VulturaInstance.systemSelectables[entities[i].entityIndex].Type;

                var distance = e.Q<Label>("distance");
                distance.text = entities[i].distance.ToString();
            } catch (ArgumentOutOfRangeException)
            {
            }

        e.RegisterCallback<ClickEvent>(ev => {
                try
                {
                    if (entities[i].entityIndex < VulturaInstance.systemSelectables.Count)
                    {
                        if (Input.GetKey("left ctrl"))
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.systemSelectables[entities[i].entityIndex], true, false);
                        }
                        else if (Input.GetKey("left alt"))
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.systemSelectables[entities[i].entityIndex], false, true);
                        }
                        else
                        {
                            VulturaInstance.selectorList.ConfirmSelection(VulturaInstance.systemSelectables[entities[i].entityIndex]);
                        }
                    }

                    SortAndPopulate();
                } catch (ArgumentOutOfRangeException)
                {
                }
            });

        try
            {
                if (entities[i].entityIndex < VulturaInstance.systemSelectables.Count)
                {
                    if (VulturaInstance.systemSelectables[entities[i].entityIndex].MainSelected)
                    {
                        e.EnableInClassList("normal", false);
                        e.EnableInClassList("main-selected-element", true);
                        e.EnableInClassList("selected-element", false);
                    }
                    else if (VulturaInstance.systemSelectables[entities[i].entityIndex].Selected)
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

        for (int i = 0; i < VulturaInstance.systemSelectables.Count; i++)
        {
            DataStruct newDistance = new DataStruct {
                entityIndex = i,
                distance = VulturaInstance.CalculateDistance(VulturaInstance.currentPlayer, VulturaInstance.systemSelectables[i].selectableObject),
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
