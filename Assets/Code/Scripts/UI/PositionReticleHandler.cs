using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PositionReticleHandler : MonoBehaviour
{
    public int entityIndex = -1;
    public GameObject text;
    

    void Update()
    {
        if (entityIndex != -1)
        {
            Vector3 playerPosition = VulturaInstance.currEntity.GetPosition() + VulturaInstance.currentPlayer.transform.position;

            Vector3 entityRealworldCoords = VulturaInstance.systemEntities[entityIndex].entity.GetPosition() - VulturaInstance.currEntity.GetPosition();

            Vector3 elementPos = Camera.main.WorldToScreenPoint(entityRealworldCoords);

            if (elementPos.z < 0 || elementPos.x < 0 || elementPos.x > Screen.width || elementPos.y < 0 || elementPos.y > Screen.height)
            {
                elementPos = new Vector3(-100, -100, 0);
            }

            Vector3 targetScreenPoint = new Vector3(Mathf.Clamp(elementPos.x, -100, Screen.width + 100), Mathf.Clamp(elementPos.y, -100, Screen.height + 100), 0);

            this.gameObject.transform.position = targetScreenPoint;

            float actualDistance = VulturaInstance.CalculateCoordinateDistance(playerPosition, VulturaInstance.systemEntities[entityIndex].entity.GetPosition());

            string distanceText;

            if (actualDistance > 50000000)
                distanceText = (actualDistance / 149597871.0f).ToString("N2") + " AU";
            else
                distanceText = actualDistance.ToString("N2") + " km";

            text.GetComponent<TextMeshProUGUI>().text = VulturaInstance.systemEntities[entityIndex].name + " (" + distanceText + ")";
        }
    }
}
