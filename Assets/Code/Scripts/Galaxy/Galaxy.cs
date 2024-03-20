using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Galaxy
{
    public Dictionary<string, StarSystem> systemList = new Dictionary<string, StarSystem>();
    public GalaxyGraph galGraph = new GalaxyGraph();

    public Galaxy(JSONGalaxy galaxy)
    {
        foreach (SystemData system in galaxy.data)
        {
            StarSystem tempSystem = new StarSystem(system.size, system.system_name, system.star_type, system.planets, system.asteroid_fields);

            systemList.Add(system.system_name, tempSystem);

            galGraph.AddNode(tempSystem);
        }
            
        foreach (SystemConnections conn in galaxy.connections)
        {
            systemList[conn.first].AddGate(new SystemGate(systemList[conn.second], new Vector3(0, 0, 0)));
            systemList[conn.second].AddGate(new SystemGate(systemList[conn.first], new Vector3(0, 0, 0)));

            galGraph.AddEdge(systemList[conn.first], systemList[conn.second]);
        }
    }
}