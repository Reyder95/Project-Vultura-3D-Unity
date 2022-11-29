using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncherFactory : BaseItemFactory
{
    public override BaseItem Create()
    {
        return new RocketLauncher(VulturaInstance.GenerateItemRarity());
    }
}
