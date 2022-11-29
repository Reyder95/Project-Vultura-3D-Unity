using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuxuryGoodsFacility : Facility
{
    public bool demand = false;

    public LuxuryGoodsFacility() : base(new FacilityItem[] { new FacilityItem(() => new LuxuryGoods(), 5) }, new FacilityItem[] { new FacilityItem(() => new FreshFood(), 7)}, "Luxury Goods Factory")
    {

    }
}
