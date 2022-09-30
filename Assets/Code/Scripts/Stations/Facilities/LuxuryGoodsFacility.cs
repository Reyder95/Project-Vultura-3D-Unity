using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuxuryGoodsFacility : Facility
{
    public string facilityName = "Luxury Goods Factory";
    public bool demand = false;

    public LuxuryGoodsFacility() : base(new FacilityItem[] { new FacilityItem(() => new LuxuryGoods(), 10000) }, new FacilityItem[] { new FacilityItem(() => new FreshFood(), 7)})
    {

    }
}
