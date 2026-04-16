using RogueLib.Enemies;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.Dungeon;

public abstract class DungeonSpawner
{
    protected HashSet<Vector2> _floor;
    protected int _level;
    protected Random _rng = new();

    protected DungeonSpawner(HashSet<Vector2> floor, int level)
    {
        _floor = floor;
        _level = level;
    }

    // ================= ABSTRACT CONTRACT =================
    public abstract List<Enemy> SpawnEnemies();
    public abstract List<Item> SpawnItems();
    public abstract Vector2 SpawnExit(List<Item> items, List<Enemy> enemies);

    // ================= SHARED UTILITY =================
    protected Vector2 RandomFloor()
        => _floor.ElementAt(_rng.Next(_floor.Count));
}

