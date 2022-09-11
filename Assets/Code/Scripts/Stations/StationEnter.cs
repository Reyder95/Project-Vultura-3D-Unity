using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles when a player enters the trigger of a station
public class StationEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == VulturaInstance.currentPlayer)
        {
            if (this.gameObject.transform.parent.tag == "Station")
            {
                this.gameObject.transform.parent.GetComponent<StationComponent>().StationEnter();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == VulturaInstance.currentPlayer)
        {
            if (this.gameObject.transform.parent.tag == "Station")
            {
                this.gameObject.transform.parent.GetComponent<StationComponent>().StationExit();
            }
        }
    }
}
