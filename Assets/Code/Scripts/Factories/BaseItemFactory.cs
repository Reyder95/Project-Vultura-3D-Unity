using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseItemFactory
{
    public GameObject prefab;

    public void myItem()
    {
        Debug.Log("Test!");
    }
}
