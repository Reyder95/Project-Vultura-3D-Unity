using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JSONDataHandler
{
    public static JSONTypes Types;
    public static JSONCategories Categories;

    public static void LoadData()
    {
        var jsonTextFile = Resources.Load<TextAsset>("JSON/Items/Types");
        Types = JsonUtility.FromJson<JSONTypes>(jsonTextFile.text);

        jsonTextFile = Resources.Load<TextAsset>("JSON/Items/Categories");
        Categories = JsonUtility.FromJson<JSONCategories>(jsonTextFile.text);
    }
}
