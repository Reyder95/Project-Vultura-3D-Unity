using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookery : Facility
{
    public bool demand = false;

    public Cookery() : base(new FacilityItem[] { new FacilityItem(() => new FreshFood(), 10) }, new FacilityItem[] { new FacilityItem(() => new FreshWater(), 4)}, "Cookery")
    {

    }
}
