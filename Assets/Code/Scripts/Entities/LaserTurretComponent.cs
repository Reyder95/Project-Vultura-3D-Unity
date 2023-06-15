using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurretComponent : TurretComponent
{
    public GameObject laserPrefab;
    public GameObject firePoint;

    private GameObject spawnedLaser;

    // Timer Variables
    float timeRemaining = 0f;
    bool timerIsRunning = false;
    bool repeatTimer = false;
    float timeCap = 0f;

    void Start()
    {
        spawnedLaser = Instantiate(laserPrefab, firePoint.transform) as GameObject;
        spawnedLaser.SetActive(false);
    }

    void Update()
    {
        if (timerIsRunning && target == null)
        {
            StopLaser();
        }

        if (timerIsRunning)
        {
            if (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                Use();

                if (repeatTimer)
                {
                    timeRemaining = timeCap;
                }
                else
                {
                    StopLaser();
                }
            }
        }

        UpdateLaser();
    }

    public void StartLaser()
    {
        if (!timerIsRunning)
        {
            timeRemaining = 5f;
            timeCap = 5f;
            timerIsRunning = true;
            repeatTimer = true;

            EnableLaser();
        }
    }

    public void StopLaser()
    {
        timeRemaining = 0f;
        timerIsRunning = false;
        repeatTimer = false;
        timeCap = 0f;

        DisableLaser();
    }

    public void Use()
    {
        if (target is AsteroidSelectable)
        {
            target.selectableObject.GetComponent<Asteroid>().Mine(VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip);
        }
    }

    void EnableLaser()
    {
        spawnedLaser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, target.selectableObject.transform.position);
        spawnedLaser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(0, firePoint.transform.position);
        spawnedLaser.SetActive(true);
    }

    void DisableLaser()
    {
        spawnedLaser.SetActive(false);
    }

    void UpdateLaser()
    {
        if (firePoint != null)
        {
            spawnedLaser.transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(0, firePoint.transform.position);
        }
    }
}
