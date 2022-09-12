using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles all functions related to ship prefabs
public class PrefabHandler : MonoBehaviour
{
    public InstantiatedShip currShip;
    public Fleet fleetAssociation;
    public ShipStats shipStats;

    public Renderer shipRenderer;

    public GameObject shipContainer;

    void Start()
    {
        shipStats = GetShipStats();
    }

    // When no player is initialized, set up the inital player.
    public void InitialPlayer()
    {
        VulturaInstance.currentPlayer = this.gameObject;
        Camera.main.gameObject.GetComponent<CameraHandler>().ReinitializeCamera(this.gameObject);
        //AddMainCamera();
        AddMovement();
    }

    private void SwitchMainCamera(GameObject oldPrefab)
    {
        Camera.main.gameObject.GetComponent<CameraHandler>().ReinitializeCamera(this.gameObject);
    }

    public void RemoveMainCamera()
    {
        this.gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
    }

    public void AddMainCamera()
    {
        this.gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
    }

    private void AddMovement()
    {
        this.gameObject.transform.gameObject.GetComponent<PlayerController>().enabled = true;
    }

    private void RemoveMovement()
    {
        this.gameObject.transform.gameObject.GetComponent<PlayerController>().enabled = false;
    }

    private void SwitchMovement(GameObject oldPrefab)
    {
        oldPrefab.GetComponent<PrefabHandler>().RemoveMovement();
        AddMovement();
    }

    public GameObject SwitchControl(GameObject oldPrefab)
    {
        
        VulturaInstance.currentPlayer = this.gameObject;
        
        SwitchMainCamera(oldPrefab);
        
        SwitchMovement(oldPrefab);
        

        return this.gameObject;
    }

    public ShipStats GetShipStats()
    {
        return this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<ShipStats>();
    }

    public void SetAssociativeData(InstantiatedShip currShip, Fleet fleet)
    {
        this.currShip = currShip;
        this.fleetAssociation = fleet;
    }

    public void SetShipColor(Color32 color)
    {
        shipRenderer.material.SetColor("_BaseColor", color);
    }

    // public void InstantiateBaseShip()
    // {
    //     currShip = new InstantiatedShip(shipStats.baseHealth, shipStats.baseArmor, shipStats.baseHull);
    // }
}
