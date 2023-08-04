using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCanvasStuff : MonoBehaviour
{
    public GameObject reticlePrefab;

    public void InstantiateReticle(int entityIndex)
    {
        GameObject newReticle = Instantiate(reticlePrefab, this.gameObject.transform);

        newReticle.GetComponent<PositionReticleHandler>().entityIndex = entityIndex;
    }
}
