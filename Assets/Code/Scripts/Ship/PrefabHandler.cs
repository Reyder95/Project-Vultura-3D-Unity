using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles all functions related to ship prefabs
public class PrefabHandler : MonoBehaviour
{
    public InstantiatedShip currShip;       // The instance and stats of this ship game object
    public Fleet fleetAssociation;          // The fleet that this ship is association
    public ShipStats shipStats;             // The base stats of this ship.

    public Renderer shipRenderer;           // -- Debug -- The renderer of this game object (to change the color)

    // Warping mechanics and booleans to determine what a ship will do depending ond distance 
    public bool warping = false;
    public bool traveling = false;
    public bool turning = false;

    public GameObject warpTarget;   // Where the ship will warp to

    void Start()
    {
        shipStats = GetShipStats();
    }

    void FixedUpdate()
    {
        // If turning is active, turn the ship towards the objective
        if (turning)
        {
            Quaternion rotTarget = Quaternion.LookRotation(warpTarget.transform.position - transform.position);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, rotTarget, 0.5f * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, rotTarget) <= 3.0f)
            {
                transform.LookAt(warpTarget.transform);
                turning = false;

                if (VulturaInstance.CalculateDistance(this.gameObject, warpTarget) < 25.0f)
                    traveling = true;
                else
                {
                    warping = true;
                    this.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
                }
            }


        }

        // If we're warping, move the ship forward to the objective
        if (warping)
        {
            // Vector3 relativePos = warpTarget.transform.position - transform.position;
            // this.gameObject.GetComponent<Rigidbody>().AddForce(1000f * relativePos.normalized);
            transform.position = Vector3.MoveTowards(transform.position, warpTarget.transform.position, 1000f * Time.deltaTime);
            if (VulturaInstance.CalculateDistance(this.gameObject, warpTarget) < 2.0f)
            {
                this.gameObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
                warping = false;
            }
        }
        // If traveling, move normally towards the objective
        else if (traveling)
        {
            this.gameObject.GetComponent<ShipMovement>().MoveShip(1.0f);
            
             if (VulturaInstance.CalculateDistance(this.gameObject, warpTarget) < 2.0f)
            {
                traveling = false;
            }
        }
    }

    // Actively cancel the warp
    public void CancelWarp()
    {
        if (turning == true)
            turning = false;
    }

    // When no player is initialized, set up the inital player.
    public void InitialPlayer()
    {
        VulturaInstance.currentPlayer = this.gameObject;
        Camera.main.gameObject.GetComponent<CameraHandler>().ReinitializeCamera(this.gameObject);
        AddInput();
        //AddMainCamera();
        AddMovement();
    }

    // Switch the main camera to this prefab from an old prefab
    private void SwitchMainCamera(GameObject oldPrefab)
    {
        Camera.main.gameObject.GetComponent<CameraHandler>().ReinitializeCamera(this.gameObject);
    }

    // Remove the main camera from this prefab
    public void RemoveMainCamera()
    {
        this.gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
    }

    // Add the main camera to this prefab
    public void AddMainCamera()
    {
        this.gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
    }

    // Add wasd movement to this ship prefab
    private void AddMovement()
    {
        this.gameObject.transform.gameObject.GetComponent<PlayerController>().enabled = true;
        this.gameObject.transform.gameObject.GetComponent<PlayerInput>().enabled = true;
    }

    //  Remove wasd movement from this ship prefab
    private void RemoveMovement()
    {
        this.gameObject.transform.gameObject.GetComponent<PlayerController>().enabled = false;
        this.gameObject.transform.gameObject.GetComponent<PlayerInput>().enabled = false;
    }

    // Add the input type to this ship prefab
    private void AddInput()
    {
        this.gameObject.GetComponent<PlayerInput>().enabled = true;
    }

    // Remove the input type from this ship prefab
    private void RemoveInput()
    {
        this.gameObject.GetComponent<PlayerInput>().enabled = false;
    }

    // Switch the input from the old ship to the new one (when switching ships)
    private void SwitchInput(GameObject oldPrefab)
    {
        oldPrefab.GetComponent<PrefabHandler>().RemoveInput();
        AddInput();
    }

    // Switch movement from an old ship to a new ship
    private void SwitchMovement(GameObject oldPrefab)
    {
        oldPrefab.GetComponent<PrefabHandler>().RemoveMovement();
        AddMovement();
    }

    // Begin the warp mechanic
    public void BeginWarp(GameObject target)
    {
        warpTarget = target;
        turning = true;
    }

    // Switch control from an old ship to the new ship
    public GameObject SwitchControl(GameObject oldPrefab)
    {
        
        VulturaInstance.currentPlayer = this.gameObject;
        
        SwitchMainCamera(oldPrefab);
        
        SwitchMovement(oldPrefab);

        SwitchMovement(oldPrefab);
        

        return this.gameObject;
    }

    // Get the ship stats from the ship component
    public ShipStats GetShipStats()
    {
        return this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<ShipStats>();
    }

    // Set the data associative 
    public void SetAssociativeData(InstantiatedShip currShip, Fleet fleet)
    {
        this.currShip = currShip;
        this.fleetAssociation = fleet;
    }

    public void SetShipColor(Color32 color)
    {
        shipRenderer.material.SetColor("_BaseColor", color);
    }
}
