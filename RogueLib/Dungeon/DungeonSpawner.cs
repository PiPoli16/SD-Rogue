using RogueLib.Enemies;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueLib.Dungeon;

/// <summary>
/// Abstract base class for dungeon content generation.
/// 
/// This class defines the contract for any procedural dungeon spawner.
/// It is responsible for generating:
/// - Enemies
/// - Items
/// - Level exit
/// 
/// Different implementations can customize difficulty, loot, and balance.
/// </summary>
public abstract class DungeonSpawner
{
    /// <summary>
    /// Set of all walkable floor tiles in the dungeon.
    /// Used as valid spawn locations for entities.
    /// </summary>
    protected HashSet<Vector2> _floor;

    /// <summary>
    /// Current dungeon level.
    /// Used for scaling difficulty and rewards.
    /// </summary>
    protected int _level;

    /// <summary>
    /// Random generator used for spawning logic.
    /// </summary>
    protected Random _rng = new();

    /// <summary>
    /// Initializes the spawner with available floor tiles and level number.
    /// </summary>
    /// <param name="floor">All walkable tiles in the map</param>
    /// <param name="level">Current dungeon level</param>
    protected DungeonSpawner(HashSet<Vector2> floor, int level)
    {
        _floor = floor;
        _level = level;
    }

    // ================= ABSTRACT CONTRACT =================

    /// <summary>
    /// Generates all enemies for the current dungeon level.
    /// </summary>
    public abstract List<Enemy> SpawnEnemies();

    /// <summary>
    /// Generates all items (loot, potions, weapons, armor, gold).
    /// </summary>
    public abstract List<Item> SpawnItems();

    /// <summary>
    /// Generates the exit position for the level.
    /// Must ensure it does not overlap with enemies or items.
    /// </summary>
    public abstract Vector2 SpawnExit(List<Item> items, List<Enemy> enemies);

    // ================= SHARED UTILITY =================

    /// <summary>
    /// Returns a random valid floor tile for spawning objects.
    /// </summary>
    protected Vector2 RandomFloor()
        => _floor.ElementAt(_rng.Next(_floor.Count));
}