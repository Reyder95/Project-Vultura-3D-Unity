using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy
{
    public List<StarSystem> systemList = new List<StarSystem>();

    public Galaxy(JSONGalaxy galaxy)
    {
        foreach (SystemData system in galaxy.data)
        {
            StarSystem tempSystem = new StarSystem(system.size, system.system_name, system.star_type, system.planets, system.asteroid_fields, system.gates);

            systemList.Add(tempSystem);
        }
    }
}