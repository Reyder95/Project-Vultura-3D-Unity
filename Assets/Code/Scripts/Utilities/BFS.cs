using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class BFS
{
    private Dictionary<StarSystem, StarSystem> previous;
    private HashSet<StarSystem> visited;
    private Queue<StarSystem> queue;
    private ManualResetEventSlim doneEvent;
    
    public BFS()
    {
        doneEvent = new ManualResetEventSlim(false);
    }

    public void FindShortestPath(GalaxyGraph graph, StarSystem startNode, StarSystem endNode)
    {
        previous = new Dictionary<StarSystem, StarSystem>();
        visited = new HashSet<StarSystem>();
        queue = new Queue<StarSystem>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            if (currentNode == endNode)
            {
                break;
            }

            foreach (var neighbor in graph.GetNeighbors(currentNode))
            {
                if (!visited.Contains(neighbor))
                {
                    previous[neighbor] = currentNode;
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        doneEvent.Set();
    }

    public List<StarSystem> GetShortestPath(StarSystem startNode, StarSystem endNode)
    {
        var shortestPath = new List<StarSystem>();

        var current = endNode;
        while (current != null)
        {
            shortestPath.Add(current);
            if (current == startNode)
            {
                break;
            }

            current = previous[current];
        }
        shortestPath.Reverse();
        return shortestPath;
    }

    public bool IsDone()
    {
        return doneEvent.IsSet;
    }
}
