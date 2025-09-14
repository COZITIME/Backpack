using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Pathfinder
{
    public const bool DoDiagonals = true;

    public static List<Vector2Int> GetPathToEntity(
        EntityTransform myTransform,
        PlayerTransform targetTransform,
        bool avoidWalls = true,
        bool avoidOtherEntities = true)
    {
        Vector2Int start = myTransform.MapPosition;
        Vector2Int end = targetTransform.MapPosition;

        return FindPath(start, end, avoidWalls, avoidOtherEntities);
    }

    private static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, bool avoidWalls, bool avoidOtherEntities)
    {
        Queue<Node> queue = new();
        queue.Enqueue(new Node(start, null));

        HashSet<Vector2Int> alreadyLookedAt = new HashSet<Vector2Int>();

        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            var neighbours = GetNeighbors(next.Position);

            foreach (var neighbour in neighbours)
            {
                if (neighbour == end)
                {
                    var targetNode = new Node(end, next);
                    return targetNode.ToPathList();
                }

                if (!alreadyLookedAt.Add(neighbour)) continue;
                if (!MapManager.Instance.IsFree(neighbour, avoidWalls, avoidOtherEntities))
                {
                    continue;
                }

                queue.Enqueue(new Node(neighbour, next));
            }
        }

        // No path found
        return new List<Vector2Int>();
    }

    private static List<Vector2Int> ReconstructPath(Node endNode)
    {
        var path = new List<Vector2Int>();
        Node current = endNode;

        while (current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }

        path.Reverse();
        return path;
    }

    private static float GetHeuristic(Vector2Int a, Vector2Int b)
    {
        // Diagonal distance
        if (DoDiagonals)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return Mathf.Max(dx, dy);
        }
        else
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

    public static bool TryGetFirstStep(EntityTransform myTransform, PlayerTransform targetTransform,
        out Vector2Int firstStep, bool avoidWalls = true, bool avoidOtherEntities = true)
    {
        firstStep = myTransform.MapPosition;
        List<Vector2Int> path = GetPathToEntity(myTransform, targetTransform, avoidWalls, avoidOtherEntities);

        if (path.Count < 2) // 0 = no path, 1 = start == end
            return false;

        firstStep = path[1]; // first step after start
        return true;
    }

    private static IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        var offsets = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        if (DoDiagonals)
        {
            offsets.AddRange(new[]
            {
                new Vector2Int(1, 1),
                new Vector2Int(-1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1)
            });
        }

        offsets = offsets.Shuffle().ToList();

        foreach (var offset in offsets)
            yield return pos + offset;
    }

    public static bool IsNeighbour(Vector2Int start, Vector2Int end)
    {
        return GetNeighbors(start).Contains(end);
    }

    private class Node
    {
        public Vector2Int Position;
        public Node Parent;

        public Node(Vector2Int position, Node parent)
        {
            Position = position;
            Parent = parent;
        }

        public List<Vector2Int> ToPathList()
        {
            var path = new List<Vector2Int>();

            var parent = this;
            int i = 0;
            while (true)
            {
                if (parent == null) break;
                path.Add(parent.Position);
                parent = parent.Parent;
                i++;
                if (i > 40)
                {
                    throw new System.Exception("Uh-Oh Stinky pathfinding got stuck");
                }
            }

            path.Reverse();
            return path;
        }
    }
}