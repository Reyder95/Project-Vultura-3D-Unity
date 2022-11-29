using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreshWaterFactory : BaseItemFactory
{
    public override BaseItem Create()
    {
        return new FreshWater();
    }
}
