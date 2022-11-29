using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    // The current player's rigidbody
    Rigidbody m_Rigidbody;
    
    // Rotation and thrust speed of the ship associated with this controller
    float rotationSpeed;
    float thrust;

    // Initialize Ship Stats

    // Start is called before the first frame update
    void Start()
    {
        // The rigidbody component obtained from the current object
        m_Rigidbody = GetComponent<Rigidbody>();

        rotationSpeed = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<ShipStats>().rotationSpeed;
        thrust = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<ShipStats>().thrust;
    }

    public void MoveShip(float verticalAxis)
    {
        try {
            m_Rigidbody.AddForce(transform.forward * verticalAxis * Time.fixedDeltaTime * thrust);  // W - S | Moves ship forward and backwards. --TODO-- Need to work on speeds via top speeds.
        }
        catch (NullReferenceException)
        {
        }
        
    }

    public void TurnShip(float horizontalAxis)
    {
        try
        {
            m_Rigidbody.AddTorque(transform.up * horizontalAxis * Time.fixedDeltaTime * rotationSpeed);     // A - D | Turns ship left and right.
        }
        catch (NullReferenceException)
        {
        }
        
    }

    public void PitchShip(float pitchAxis)
    {
        try
        {
            m_Rigidbody.AddTorque(-transform.right * pitchAxis * Time.fixedDeltaTime * rotationSpeed);      // R - F | Turns ship up and down.
        }
        catch (NullReferenceException)
        {
        }

        
    }

    public void RollShip(float rollAxis)
    {
        try
        {
            m_Rigidbody.AddTorque(-transform.forward * rollAxis * Time.fixedDeltaTime * rotationSpeed);     // Q - E | Rolls ship left and right
        }
        catch (NullReferenceException)
        {
        }
        
    }
}
