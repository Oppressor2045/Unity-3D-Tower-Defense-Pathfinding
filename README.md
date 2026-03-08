# Unity 3D Tower Defense Pathfinding

A research-oriented reference on pathfinding algorithms for real-time strategy and tower defense games.
This repository documents definitions, real-world implementations, and a personal research roadmap.

---

## What is Pathfinding?

Pathfinding is the computational problem of finding a route between two points in a space,
while avoiding obstacles. In games, this means getting an agent from point A to point B
through a map that may have walls, terrain, or dynamically placed objects like towers.

The challenge is not just finding *a* path — it's finding the *optimal* path efficiently,
even as the environment changes in real time.

---

## Algorithms

### A* (A-Star)

The most widely used pathfinding algorithm in games. Combines the actual cost from the
start node (G cost) with an estimated cost to the goal (H cost, the heuristic) to always
expand the most promising node first.

```
F = G + H
     │   │
     │   └── Heuristic (estimated distance to goal)
     └─────── Actual cost from start
```

Best for: single-agent, known map, dynamic obstacle support.
Used in: this project, Warcraft III, Age of Empires, most RTS games.

### Dijkstra

Explores all directions equally with no heuristic. Guarantees the shortest path but is
slower than A* because it does not bias toward the goal.

Best for: pre-computing all distances from a single source.
Used in: navigation systems, network routing.

### BFS (Breadth-First Search)

Explores layer by layer from the start. Optimal only for unweighted graphs (all edges cost the same).
Simple to implement but not suitable for large or weighted maps.

Best for: small grids, unweighted maps, maze solving.

### Flow Field

Instead of calculating a separate path per agent, a flow field pre-computes a direction vector
for every cell pointing toward the goal. All agents just follow the vector at their current cell.

Best for: large numbers of agents sharing the same destination.
Used in: Planetary Annihilation, Starcraft II (crowd movement).

---

## Real-World Implementations

Projects and resources that demonstrate pathfinding in practice:

- [Sebastian Lague — A* Pathfinding](https://github.com/SebLague/Pathfinding)
  The most referenced Unity A* tutorial series. Clean grid-based implementation with heap optimization.

- [Recast Navigation](https://github.com/recastnavigation/recastnavigation)
  Industry-standard NavMesh generation used in Unity, Unreal, and CryEngine.

- [EpPathFinding.cs](https://github.com/juhgiyo/EpPathFinding.cs)
  C# pathfinding library with Jump Point Search, BFS, and A* on a grid.

- [Roy Triesscheijn — Flow Fields](http://www.gamedev.net/blog/866/entry-2250317-flowfields/)
  Detailed breakdown of flow field pathfinding used in RTS games.

- [Amit Patel — Red Blob Games](https://www.redblobgames.com/pathfinding/a-star/introduction.html)
  The definitive visual explainer for A*, Dijkstra, and BFS. Highly recommended reading.

---

## This Implementation

Grid-based A* with dynamic recalculation for a 3D tower defense game built in Unity.

```
GridMap           — 2D grid, obstacle detection via Physics.CheckSphere
AStarPathfinder   — A* with Manhattan distance heuristic (4-directional)
PathManager       — broadcasts path updates via C# event
EnemyMover        — follows waypoints, rerouts on grid change
```

When a tower is placed, `PathManager.RequestPathRecalculation()` refreshes the affected
node and fires `OnPathUpdated` — all active enemies reroute from their current position.

---

## Research Roadmap

### Near-term
- Replace `List<Node>` open set with a **min-heap (priority queue)** for O(log n) performance
- Add **8-directional movement** with diagonal cost (√2 ≈ 1.414)
- Implement **path smoothing** — remove unnecessary waypoints with line-of-sight checks

### Mid-term
- Implement **Jump Point Search (JPS)** — A* variant that skips redundant nodes on uniform grids
- Explore **Flow Field pathfinding** for handling 50+ enemies sharing one destination
- Research **Hierarchical Pathfinding A* (HPA*)** for large maps divided into clusters

### Long-term
- Study **NavMesh** generation and how Unity's built-in NavMesh agent works internally
- Investigate **multi-agent coordination** — preventing agents from overlapping paths
- Research **machine learning approaches** to pathfinding (Deep Q-Network navigation)

---

## References

- Hart, P. E., Nilsson, N. J., & Raphael, B. (1968). *A Formal Basis for the Heuristic Determination of Minimum Cost Paths.* IEEE Transactions on Systems Science and Cybernetics.
- Amit Patel. *Introduction to A*.  https://www.redblobgames.com/pathfinding/a-star/introduction.html
- Sebastian Lague. *A* Pathfinding Tutorial.* https://www.youtube.com/watch?v=-L-WgKMFuhE

---

*For educational and research purposes.*
