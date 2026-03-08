# Changelog

All notable changes to this project will be documented in this file.

Format: [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

---

## [1.0.0] — 2026-03-08

### Added

#### Pathfinding
- `Node.cs` — grid cell data structure with GCost, HCost, FCost and Parent node
- `GridMap.cs` — 2D grid generation with Physics.CheckSphere obstacle detection
- `GridMap.cs` — RefreshNode() for single-node walkability update on tower placement
- `GridMap.cs` — OnDrawGizmos debug visualization in Unity editor
- `AStarPathfinder.cs` — A* algorithm with open/closed set and Manhattan distance heuristic
- `AStarPathfinder.cs` — RetracePath() via Parent node chain
- `EnemyMover.cs` — coroutine-based waypoint movement with rotation
- `EnemyMover.cs` — dynamic path update via PathManager.OnPathUpdated event
- `PathManager.cs` — central pathfinding controller with OnPathUpdated broadcast
- `PathManager.cs` — RequestPathRecalculation() for tower place and remove events

#### Project
- `README.md` — setup guide, script reference, project structure
- `CHANGELOG.md` — this file

---

## [Unreleased]

### Planned

| Type | Feature |
|------|---------|
| `feat` | Object pooling for enemies |
| `feat` | Multiple enemy lanes |
| `feat` | Path visualization via LineRenderer |
| `feat` | Diagonal movement support (8-directional grid) |
| `fix`  | Path blocked edge case — notify player when no path exists |
| `perf` | Priority queue (min-heap) to replace List open set |
