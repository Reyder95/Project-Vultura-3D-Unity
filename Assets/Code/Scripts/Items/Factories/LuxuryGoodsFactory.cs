using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuxuryGoodsFactory : BaseItemFactory
{
    public override BaseItem Create()
    {
        return new LuxuryGoods();
    }
}
