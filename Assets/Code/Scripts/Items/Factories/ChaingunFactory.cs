using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaingunFactory : BaseItemFactory
{
    public override BaseItem Create()
    {
        return new Chaingun(VulturaInstance.GenerateItemRarity());
    }
}
