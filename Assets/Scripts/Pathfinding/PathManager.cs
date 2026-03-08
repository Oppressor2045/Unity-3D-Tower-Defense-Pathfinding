using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Pathfinding
{
    /// <summary>
    /// Central manager for pathfinding.
    /// - Owns the single shared path (start → end)
    /// - Broadcasts OnPathUpdated when the grid changes
    /// - Call RequestPathRecalculation() after placing or removing a tower
    /// </summary>
    public class PathManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GridMap        grid;
        [SerializeField] private AStarPathfinder pathfinder;

        [Header("Path Endpoints")]
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform endPoint;

        /// <summary>Fired whenever a new path is calculated.</summary>
        public event Action<List<Vector3>> OnPathUpdated;

        private List<Vector3> _currentPath = new List<Vector3>();

        public List<Vector3> CurrentPath => _currentPath;

        // ─────────────────────────────────────────────────────────────────────────
        // Unity lifecycle
        // ─────────────────────────────────────────────────────────────────────────

        private void Start() => RecalculatePath();

        // ─────────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Call this after placing or removing a tower.
        /// Refreshes the affected grid node and recalculates all enemy paths.
        /// </summary>
        public void RequestPathRecalculation(Vector3 changedWorldPos)
        {
            // Refresh the node where the tower was placed/removed
            Node node = grid.NodeFromWorld(changedWorldPos);
            if (node != null)
                grid.RefreshNode(node.GridX, node.GridY);

            RecalculatePath();
        }

        /// <summary>Forces a full path recalculation without refreshing a specific node.</summary>
        public void RequestPathRecalculation() => RecalculatePath();

        // ─────────────────────────────────────────────────────────────────────────
        // Internal
        // ─────────────────────────────────────────────────────────────────────────

        private void RecalculatePath()
        {
            if (startPoint == null || endPoint == null)
            {
                Debug.LogError("[PathManager] Start or End point is not assigned.");
                return;
            }

            _currentPath = pathfinder.FindPath(startPoint.position, endPoint.position);

            if (_currentPath.Count == 0)
                Debug.LogWarning("[PathManager] No valid path found between start and end.");

            // Notify all enemies to update their path
            OnPathUpdated?.Invoke(_currentPath);
        }
    }
}
