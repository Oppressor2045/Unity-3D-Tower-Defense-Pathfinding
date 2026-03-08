using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Pathfinding
{
    /// <summary>
    /// Generates and manages the 2D grid used for A* pathfinding.
    /// Attach this to an empty GameObject in the scene.
    /// </summary>
    public class GridMap : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20);
        [SerializeField] private float      cellSize  = 1f;

        [Header("Obstacle Detection")]
        [SerializeField] private LayerMask  obstacleLayer;
        [SerializeField] private float      obstacleCheckRadius = 0.4f;

        [Header("Debug")]
        [SerializeField] private bool drawGizmos = true;

        private Node[,] _grid;

        public int   Width    => gridSize.x;
        public int   Height   => gridSize.y;
        public float CellSize => cellSize;

        private Vector3 Origin =>
            transform.position - new Vector3(gridSize.x * cellSize * 0.5f, 0f, gridSize.y * cellSize * 0.5f);

        // ─────────────────────────────────────────────────────────────────────────
        // Unity lifecycle
        // ─────────────────────────────────────────────────────────────────────────

        private void Awake() => BuildGrid();

        // ─────────────────────────────────────────────────────────────────────────
        // Grid construction
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>Builds the entire grid from scratch.</summary>
        public void BuildGrid()
        {
            _grid = new Node[gridSize.x, gridSize.y];

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3 worldPos = NodeToWorld(x, y);
                    bool walkable    = !Physics.CheckSphere(worldPos, obstacleCheckRadius, obstacleLayer);
                    _grid[x, y]      = new Node(x, y, worldPos, walkable);
                }
            }
        }

        /// <summary>
        /// Refreshes walkability for a single node.
        /// Call this after placing or removing a tower.
        /// </summary>
        public void RefreshNode(int x, int y)
        {
            if (!InBounds(x, y)) return;
            Vector3 worldPos     = NodeToWorld(x, y);
            _grid[x, y].Walkable = !Physics.CheckSphere(worldPos, obstacleCheckRadius, obstacleLayer);
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Node access
        // ─────────────────────────────────────────────────────────────────────────

        public Node GetNode(int x, int y) => InBounds(x, y) ? _grid[x, y] : null;

        /// <summary>Returns the grid node closest to the given world position.</summary>
        public Node NodeFromWorld(Vector3 worldPos)
        {
            Vector3 local = worldPos - Origin;
            int x = Mathf.Clamp(Mathf.FloorToInt(local.x / cellSize), 0, gridSize.x - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(local.z / cellSize), 0, gridSize.y - 1);
            return _grid[x, y];
        }

        /// <summary>Returns walkable neighbours of a node (4-directional).</summary>
        public List<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>(4);

            int[] dx = {  0,  0, -1,  1 };
            int[] dy = { -1,  1,  0,  0 };

            for (int i = 0; i < 4; i++)
            {
                int nx = node.GridX + dx[i];
                int ny = node.GridY + dy[i];

                if (InBounds(nx, ny) && _grid[nx, ny].Walkable)
                    neighbours.Add(_grid[nx, ny]);
            }

            return neighbours;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────────

        private Vector3 NodeToWorld(int x, int y) =>
            Origin + new Vector3(x * cellSize + cellSize * 0.5f, 0f, y * cellSize + cellSize * 0.5f);

        private bool InBounds(int x, int y) =>
            x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;

        // ─────────────────────────────────────────────────────────────────────────
        // Gizmos
        // ─────────────────────────────────────────────────────────────────────────

        private void OnDrawGizmos()
        {
            if (!drawGizmos || _grid == null) return;

            foreach (Node node in _grid)
            {
                Gizmos.color = node.Walkable
                    ? new Color(1f, 1f, 1f, 0.08f)
                    : new Color(1f, 0f, 0f, 0.35f);
                Gizmos.DrawCube(node.WorldPosition, Vector3.one * (cellSize - 0.05f));
            }
        }
    }
}
