using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoExpanderFactory : BaseItemFactory
{
    public override BaseItem Create()
    {
        return new CargoExpander(VulturaInstance.GenerateItemRarity());
    }
}
