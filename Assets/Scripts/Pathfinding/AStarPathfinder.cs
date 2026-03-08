using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Pathfinding
{
    /// <summary>
    /// A* pathfinder. Returns a list of world-space waypoints from start to end.
    /// Supports dynamic recalculation — call FindPath() again after grid changes.
    /// </summary>
    public class AStarPathfinder : MonoBehaviour
    {
        [SerializeField] private GridMap grid;

        // ─────────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Finds a path between two world positions.
        /// Returns an empty list if no path exists.
        /// </summary>
        public List<Vector3> FindPath(Vector3 startWorld, Vector3 endWorld)
        {
            Node startNode = grid.NodeFromWorld(startWorld);
            Node endNode   = grid.NodeFromWorld(endWorld);

            if (startNode == null || endNode == null) return new List<Vector3>();
            if (!endNode.Walkable)                    return new List<Vector3>();

            ResetGrid();

            // Open set — nodes to evaluate
            var openSet   = new List<Node> { startNode };
            // Closed set — nodes already evaluated
            var closedSet = new HashSet<Node>();

            while (openSet.Count > 0)
            {
                Node current = GetLowestFCost(openSet);

                if (current == endNode)
                    return RetracePath(startNode, endNode);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (Node neighbour in grid.GetNeighbours(current))
                {
                    if (closedSet.Contains(neighbour)) continue;

                    int tentativeG = current.GCost + GetDistance(current, neighbour);

                    if (tentativeG < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost  = tentativeG;
                        neighbour.HCost  = GetDistance(neighbour, endNode);
                        neighbour.Parent = current;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            // No path found
            Debug.LogWarning("[AStarPathfinder] No path found.");
            return new List<Vector3>();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Internal helpers
        // ─────────────────────────────────────────────────────────────────────────

        private void ResetGrid()
        {
            for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                    grid.GetNode(x, y)?.Reset();
        }

        private List<Vector3> RetracePath(Node start, Node end)
        {
            var path    = new List<Node>();
            Node current = end;

            while (current != start)
            {
                path.Add(current);
                current = current.Parent;
            }

            path.Reverse();

            var worldPath = new List<Vector3>(path.Count);
            foreach (Node node in path)
                worldPath.Add(node.WorldPosition);

            return worldPath;
        }

        private Node GetLowestFCost(List<Node> list)
        {
            Node lowest = list[0];
            foreach (Node node in list)
            {
                if (node.FCost < lowest.FCost ||
                    (node.FCost == lowest.FCost && node.HCost < lowest.HCost))
                    lowest = node;
            }
            return lowest;
        }

        // Manhattan distance (4-directional grid)
        private int GetDistance(Node a, Node b) =>
            Mathf.Abs(a.GridX - b.GridX) + Mathf.Abs(a.GridY - b.GridY);
    }
}
