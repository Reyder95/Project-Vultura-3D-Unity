using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A mining station. The goal of this station is to have high quality ore and rudimentary industrial equipment ready for purchase. Each mining station has bonuses to industrial / mining based facilities.
public class MiningStation : BaseStation
{
    public MiningStation(string stationName, string stationType, string faction) : base(stationName, stationType, faction)
    {
    }
    
    public override void InitializeStation()
    {
        base.InitializeStation();   // Let the base station code initialize first.
        Debug.Log("Mining Station Initialized");
    }
}
