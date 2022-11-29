using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshTimer : MonoBehaviour
{

    public static RefreshTimer Instance { get; private set; }

    public float timeRemaining = 0f;
    public bool timerIsRunning = false;
    public bool repeatTimer = false;

    public float timeCap = 0f;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                OnRefreshComplete();
                if (repeatTimer)
                {
                    timeRemaining = timeCap;
                }
                else
                    StopTimer();
            }
        }
    }

    public void StartTimer(float length, bool repeat = false)
    {
        if (!timerIsRunning)
        {
            timeRemaining = length;
            timeCap = length;
            timerIsRunning = true;
            repeatTimer = repeat;
        }
        
    }

    public void StopTimer()
    {
        timeRemaining = 0f;
        timerIsRunning = false;
        repeatTimer = false;
        timeCap = 0f;
    }

    public void OnRefreshComplete()
    {
        foreach (BaseStation station in Game.Instance.stations)
        {
            station.RunProductionChain();
        }
    }
}
