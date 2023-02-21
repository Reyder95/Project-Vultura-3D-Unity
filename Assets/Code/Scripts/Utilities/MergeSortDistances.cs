using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MergeSortDistances
{
    public static List<BaseSelectable> MergeSort(List<BaseSelectable> selectables)
    {
        if (selectables.Count <= 1)
            return selectables;

        List<BaseSelectable> left = new List<BaseSelectable>();
        List<BaseSelectable> right = new List<BaseSelectable>();

        int middle = selectables.Count / 2;

        for (int i = 0; i < middle; i++)
        {
            left.Add(selectables[i]);
        }

        for (int i = middle; i < selectables.Count; i++)
        {
            right.Add(selectables[i]);
        }

        left = MergeSort(left);
        right = MergeSort(right);
        return Merge(left, right);
    }

    public static List<BaseSelectable> Merge(List<BaseSelectable> left, List<BaseSelectable> right)
    {
        List<BaseSelectable> result = new List<BaseSelectable>();

        while ((left.Count > 0 || right.Count > 0))
        {
            if (left.Count > 0 && right.Count > 0)
            {
                if (VulturaInstance.CalculateDistance(VulturaInstance.currentPlayer, left.First().selectableObject) <= VulturaInstance.CalculateDistance(VulturaInstance.currentPlayer, right.First().selectableObject))
                {
                    result.Add(left.First());
                    left.Remove(left.First());
                }
                else
                {
                    result.Add(right.First());
                    right.Remove(right.First());
                }
            }
            else if (left.Count > 0)
            {
                result.Add(left.First());
                left.Remove(left.First());
            }
            else if (right.Count > 0)
            {
                result.Add(right.First());

                right.Remove(right.First());
            }
        }
        return result;
    }
}
