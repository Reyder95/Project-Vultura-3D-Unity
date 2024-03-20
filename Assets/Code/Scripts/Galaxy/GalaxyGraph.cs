using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyGraph
{
    private Dictionary<StarSystem, List<StarSystem>> adjacencyList;
    int vertices = 0;

    public GalaxyGraph()
    {
        adjacencyList = new Dictionary<StarSystem, List<StarSystem>>();
    }

    public void AddNode(StarSystem node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = new List<StarSystem>();
            vertices++;
        }
    }

    public void AddEdge(StarSystem node1, StarSystem node2)
    {
        if (!adjacencyList.ContainsKey(node1))
        {
            AddNode(node1);
        }

        if (!adjacencyList.ContainsKey(node2))
        {
            AddNode(node2);
        }

        adjacencyList[node1].Add(node2);
        adjacencyList[node2].Add(node1);
    }

    public List<StarSystem> GetNeighbors(StarSystem node)
    {
        if (adjacencyList.ContainsKey(node))
        {
            return adjacencyList[node];
        }
        else
        {
            return new List<StarSystem>();
        }
    }

    public bool HasNode(StarSystem node)
    {
        return adjacencyList.ContainsKey(node);
    }

    public bool HasEdge(StarSystem node1, StarSystem node2) 
    { 
        return adjacencyList.ContainsKey(node1) && adjacencyList[node1].Contains(node2);
    }

    public void BFS(StarSystem startNode)
    {
        Queue<StarSystem> queue = new Queue<StarSystem>();
        Dictionary<StarSystem, bool> visited = new Dictionary<StarSystem, bool>();

        foreach (StarSystem node in adjacencyList.Keys)
        {
            visited[node] = false;
        }

        visited[startNode] = true;
        queue.Enqueue(startNode);

        int count = 0;

        while (queue.Count != 0 || count > 10)
        {
            StarSystem currentNode = queue.Dequeue();
            Debug.Log("Current Node: " + currentNode.system_name);


            foreach (StarSystem neighbor in adjacencyList[currentNode])
            {
                Debug.Log(neighbor.system_name);
                if (!visited[neighbor])
                {
                    visited[neighbor] = true;
                    queue.Enqueue(neighbor);
                }

            }

            count++;
        }
    }
}
