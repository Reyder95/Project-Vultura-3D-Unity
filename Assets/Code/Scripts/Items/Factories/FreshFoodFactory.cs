using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreshFoodFactory : BaseItemFactory
{
    public override BaseItem Create()
    {
        return new FreshFood();
    }
}
