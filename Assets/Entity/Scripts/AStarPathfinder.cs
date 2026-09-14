using System;
using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    // no built in cost function as i try to figure out A*
    public static List<AINode> FindDumbPath(AINode start, AINode goal)
    {
        if (start == null || goal == null) return null;
        if (start == goal) return new List<AINode> {start};

        var open = new List<AINode> {start}; // explorable nodes
        var path = new Dictionary<AINode, AINode>(); // path to reconstruct and return

        var gScore = new Dictionary<AINode, float>() { [start] = 0f }; // cost through closed nodes to this point
        var fScore = new Dictionary<AINode, float>() { [start] = Euclidean(start, goal) }; // gScore + heuristic

        var closed = new HashSet<AINode>(); // nodes explored

        while (open.Count > 0)
        {
            AINode current = LowestF(open, fScore);

            if (current == goal)
            {
                return ReconstructPath(path, current);
            }

            open.Remove(current);
            closed.Add(current);

            foreach (var edge in current.Edges)
            {
                AINode neighbor = edge.Target;
                if (closed.Contains(neighbor)) continue;

                float newG = gScore[current] + edge.Distance; // swap this part out with a more advanced cost function as needed!

                if (!gScore.TryGetValue(neighbor, out float oldG) || newG < oldG )
                {
                    path[neighbor] = current;
                    gScore[neighbor] = newG;
                    fScore[neighbor] = newG + Euclidean(neighbor, goal);

                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }
            }
        }
        
        return null;
    }

    public static List<AINode> FindPath(AINode start, AINode goal, Func<AINode, AINode, float, float> costFn = null)
    {
        if (start == null || goal == null) return null;
        if (start == goal) return new List<AINode> {start};

        costFn ??= (from, to, dist) => dist;

        var open = new List<AINode> {start}; // explorable nodes
        var path = new Dictionary<AINode, AINode>(); // path to reconstruct and return

        var gScore = new Dictionary<AINode, float>() { [start] = 0f }; // cost through closed nodes to this point
        var fScore = new Dictionary<AINode, float>() { [start] = Euclidean(start, goal) }; // gScore + heuristic

        var closed = new HashSet<AINode>(); // nodes explored

        while (open.Count > 0)
        {
            AINode current = LowestF(open, fScore);

            if (current == goal)
            {
                return ReconstructPath(path, current);
            }

            open.Remove(current);
            closed.Add(current);

            foreach (var edge in current.Edges)
            {
                AINode neighbor = edge.Target;
                if (closed.Contains(neighbor)) continue;

                float newG = gScore[current] + costFn(current, neighbor, edge.Distance); // swap this part out with a more advanced cost function as needed!

                if (!gScore.TryGetValue(neighbor, out float oldG) || newG < oldG )
                {
                    path[neighbor] = current;
                    gScore[neighbor] = newG;
                    fScore[neighbor] = newG + Euclidean(neighbor, goal);

                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor);
                    }
                }
            }
        }
        
        return null;
    }

    static float Euclidean(AINode a, AINode b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    static AINode LowestF(List<AINode> open, Dictionary<AINode, float> fScore)
    {
        AINode best = open[0];
        float bestScore = fScore[best];

        for (int i = 1; i < open.Count; i++)
        {
            if (fScore[open[i]] < bestScore)
            {
                best = open[i];
                bestScore = fScore[best];
            }
        }

        return best;
    }

    static List<AINode> ReconstructPath(Dictionary<AINode, AINode> path, AINode current)
    {
        var reversed = new List<AINode> { current };
        while (path.TryGetValue(current, out var prev))
        {
            current = prev;
            reversed.Add(current);
        }
        reversed.Reverse();
        return reversed;
    }
}
