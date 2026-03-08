using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Pathfinding
{
    /// <summary>
    /// Moves an enemy along a path produced by AStarPathfinder.
    /// Subscribes to PathManager to receive dynamic path updates.
    /// </summary>
    public class EnemyMover : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed   = 3f;
        [SerializeField] private float waypointTolerance = 0.1f;

        [Header("References")]
        [SerializeField] private PathManager pathManager;

        private List<Vector3> _path = new List<Vector3>();
        private int           _waypointIndex;
        private Coroutine     _moveCoroutine;

        // ─────────────────────────────────────────────────────────────────────────
        // Unity lifecycle
        // ─────────────────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            if (pathManager != null)
                pathManager.OnPathUpdated += OnPathUpdated;
        }

        private void OnDisable()
        {
            if (pathManager != null)
                pathManager.OnPathUpdated -= OnPathUpdated;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>Starts moving along a new path immediately.</summary>
        public void SetPath(List<Vector3> newPath)
        {
            if (newPath == null || newPath.Count == 0) return;

            _path          = newPath;
            _waypointIndex = 0;

            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(FollowPath());
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Path following
        // ─────────────────────────────────────────────────────────────────────────

        private IEnumerator FollowPath()
        {
            while (_waypointIndex < _path.Count)
            {
                Vector3 target = _path[_waypointIndex];
                target.y = transform.position.y; // Keep y-axis locked

                while (Vector3.Distance(transform.position, target) > waypointTolerance)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position, target, moveSpeed * Time.deltaTime);

                    // Face movement direction
                    Vector3 dir = (target - transform.position).normalized;
                    if (dir != Vector3.zero)
                        transform.rotation = Quaternion.LookRotation(dir);

                    yield return null;
                }

                _waypointIndex++;
            }

            // Reached the end — notify or destroy
            OnReachedEnd();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Dynamic recalculation
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called by PathManager when the grid changes (tower placed/removed).
        /// Recalculates the path from the enemy's current position.
        /// </summary>
        private void OnPathUpdated(List<Vector3> newPath)
        {
            if (newPath == null || newPath.Count == 0)
            {
                Debug.LogWarning($"[EnemyMover] {name}: No valid path after update.");
                return;
            }

            SetPath(newPath);
        }

        private void OnReachedEnd()
        {
            // TODO: Deal damage to base, then return to pool
            gameObject.SetActive(false);
        }
    }
}
