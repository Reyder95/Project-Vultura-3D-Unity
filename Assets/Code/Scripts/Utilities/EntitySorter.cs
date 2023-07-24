using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Threading.Tasks;
using UnityEngine.Events;

public class EntitySorter : MonoBehaviour
{
    public static EntitySorter Instance {get; private set;}

    public UnityAction selectionChanged;

    public struct DataStruct {
        public int entityIndex;
        public float distance;
    }

    public DataStruct[] entities;
    public DataStruct[] subEntities;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        selectionChanged = new UnityAction(SortAndPopulate);

        InvokeRepeating("SortAndPopulate", 0f, 2.0f);
    }

    void OnEnable()
    {
        EventManager.StartListening("Selection Changed", selectionChanged);
    }

    void OnDisable()
    {
        EventManager.StopListening("Selection Changed", selectionChanged);
    }

    public async void SortAndPopulate()
    {
        NativeList<DataStruct> newListEntities = new NativeList<DataStruct>(Allocator.TempJob);

        for (int i = 0; i < VulturaInstance.systemSelectables.Count; i++)
        {
            DataStruct newDistance = new DataStruct {
                entityIndex = i,
                distance = VulturaInstance.CalculateDistance(VulturaInstance.currentPlayer, VulturaInstance.systemSelectables[i].selectableObject),
            };

            newListEntities.Add(newDistance);
        }

        var result = await Task.Run(() => {
            newListEntities.Sort(new EntitySort());

            return newListEntities;
        });

        entities = newListEntities.ToArray();
        
        newListEntities.Dispose();

        SortSubList();
        EventManager.TriggerEvent("RefreshList");

    }

    public async void SortSubList()
    {
        NativeList<DataStruct> newListSubEntities = new NativeList<DataStruct>(Allocator.TempJob);

        for (int i = 0; i < VulturaInstance.fleetSelectables.Count; i++)
        {
            DataStruct newDistance = new DataStruct {
                entityIndex = i,
                distance = VulturaInstance.CalculateDistance(VulturaInstance.currentPlayer, VulturaInstance.fleetSelectables[i].selectableObject),
            };

            newListSubEntities.Add(newDistance);
        }

        var resultSub = await Task.Run(() => {
            newListSubEntities.Sort(new EntitySort());

            return newListSubEntities;
        });

        subEntities = newListSubEntities.ToArray();

        newListSubEntities.Dispose();

        EventManager.TriggerEvent("RefreshList");
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
