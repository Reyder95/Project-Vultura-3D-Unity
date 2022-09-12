using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles all functions related to ship prefabs
public class PrefabHandler : MonoBehaviour
{
    public InstantiatedShip currShip;
    public Fleet fleetAssociation;
    public ShipStats shipStats;

    public Renderer shipRenderer;

    public GameObject shipContainer;

    public bool warping = false;
    public bool traveling = false;
    public bool turning = false;

    public GameObject warpTarget;

    void Start()
    {
        shipStats = GetShipStats();
    }

    void FixedUpdate()
    {
        if (turning)
        {
            Quaternion rotTarget = Quaternion.LookRotation(warpTarget.transform.position - transform.position);
            this.transform.rotation = Quaternion.Slerp(transform.rotation, rotTarget, 0.5f * Time.deltaTime);

            // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(98f, ), 100 * Time.deltaTime);
            Debug.Log(Quaternion.Angle(transform.rotation, rotTarget));

            if (Quaternion.Angle(transform.rotation, rotTarget) <= 3.0f)
            {
                transform.LookAt(warpTarget.transform);
                turning = false;

                if (VulturaInstance.CalculateDistance(this.gameObject, warpTarget) < 25.0f)
                    traveling = true;
                else
                    warping = true;
            }


        }

        if (warping)
        {
            // Vector3 relativePos = warpTarget.transform.position - transform.position;
            // this.gameObject.GetComponent<Rigidbody>().AddForce(1000f * relativePos.normalized);
            transform.position = Vector3.MoveTowards(transform.position, warpTarget.transform.position, 1000f * Time.deltaTime);
            if (VulturaInstance.CalculateDistance(this.gameObject, warpTarget) < 100f)
            {
                warping = false;
                Debug.Log("Done!");
            }
        }
        else if (traveling)
        {
            this.gameObject.GetComponent<ShipMovement>().MoveShip(1.0f);
            
             if (VulturaInstance.CalculateDistance(this.gameObject, warpTarget) < 2.0f)
            {
                traveling = false;
                Debug.Log("Done!");
            }
        }
    }

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

    private void AddInput()
    {
        this.gameObject.GetComponent<PlayerInput>().enabled = true;
    }

    private void RemoveInput()
    {
        this.gameObject.GetComponent<PlayerInput>().enabled = false;
    }

    private void SwitchInput(GameObject oldPrefab)
    {
        oldPrefab.GetComponent<PrefabHandler>().RemoveInput();
        AddInput();
    }

    private void SwitchMovement(GameObject oldPrefab)
    {
        oldPrefab.GetComponent<PrefabHandler>().RemoveMovement();
        AddMovement();
    }

    public void BeginWarp(GameObject target)
    {
        warpTarget = target;
        turning = true;
    }

    public GameObject SwitchControl(GameObject oldPrefab)
    {
        
        VulturaInstance.currentPlayer = this.gameObject;
        
        SwitchMainCamera(oldPrefab);
        
        SwitchMovement(oldPrefab);

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
