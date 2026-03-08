using UnityEngine;

namespace TowerDefense.Pathfinding
{
    /// <summary>
    /// Represents a single cell in the pathfinding grid.
    /// </summary>
    public class Node
    {
        // Grid position
        public int GridX { get; }
        public int GridY { get; }

        // World position (center of this cell)
        public Vector3 WorldPosition { get; }

        // Whether an enemy can walk through this node
        public bool Walkable { get; set; }

        // A* cost values
        public int GCost { get; set; }   // Cost from start node
        public int HCost { get; set; }   // Heuristic cost to end node
        public int FCost => GCost + HCost;

        // Parent node — used to reconstruct the path
        public Node Parent { get; set; }

        public Node(int gridX, int gridY, Vector3 worldPosition, bool walkable)
        {
            GridX         = gridX;
            GridY         = gridY;
            WorldPosition = worldPosition;
            Walkable      = walkable;
        }

        public void Reset()
        {
            GCost  = 0;
            HCost  = 0;
            Parent = null;
        }
    }
}
