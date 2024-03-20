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

        for (int i = 0; i < VulturaInstance.systemEntities.Count; i++)
        {
            Vector3 playerPosition = VulturaInstance.currEntity.GetPosition() + VulturaInstance.currentPlayer.transform.position;

            float actualDistance = VulturaInstance.CalculateCoordinateDistance(playerPosition, VulturaInstance.systemEntities[i].entity.GetPosition());

            if (VulturaInstance.systemEntities[i].entity.entity != null)
                if (VulturaInstance.systemEntities[i].entity.entity.GetType() == typeof(InstantiatedShip))
                {
                    actualDistance = VulturaInstance.CalculateCoordinateDistance(VulturaInstance.currentPlayer.transform.position, VulturaInstance.systemEntities[i].entity.GetPosition());
                }

            DataStruct newDistance = new DataStruct {
                entityIndex = i,
                distance = actualDistance,
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

        for (int i = 0; i < VulturaInstance.subEntities.Count; i++)
        {
            DataStruct newDistance;
            if (VulturaInstance.subEntities[i].entity.entity != null)
            {
                newDistance = new DataStruct {
                    entityIndex = i,
                    distance = VulturaInstance.CalculateDistance(VulturaInstance.currentPlayer, VulturaInstance.subEntities[i].entity.entity.selectableObject),
                };
            }
            else
            {
                Vector3 playerPosition = VulturaInstance.currEntity.GetPosition() + VulturaInstance.currentPlayer.transform.position;

                float actualDistance = VulturaInstance.CalculateCoordinateDistance(playerPosition, VulturaInstance.subEntities[i].entity.GetPosition());

                newDistance = new DataStruct {
                    entityIndex = i,
                    distance = actualDistance
                };
            }

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