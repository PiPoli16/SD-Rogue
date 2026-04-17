using RogueLib.items;
using RogueLib.Enemies;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueLib.Dungeon;

/// <summary>
/// Default implementation of the DungeonSpawner.
/// 
/// Responsible for generating all procedural content for a level:
/// - Enemies
/// - Items (weapons, armor, potions, gold)
/// - Exit position
/// 
/// This class defines the game balance and scaling rules per level.
/// </summary>
public class DefaultDungeonSpawner : DungeonSpawner
{
    public DefaultDungeonSpawner(HashSet<Vector2> floor, int level)
        : base(floor, level) { }

    /// <summary>
    /// Spawns enemies based on dungeon level scaling.
    /// 
    /// Enemy stats increase with level:
    /// - AttackPower increases by (level - 1) * 10
    /// - Troll count increases as level increases
    /// </summary>
    public override List<Enemy> SpawnEnemies()
    {
        var enemies = new List<Enemy>();

        // Global stat scaling applied to all enemies
        int statIncrease = (_level - 1) * 10;

        // Fixed enemy counts per type (except trolls scale with level)
        int goblins = 2;
        int orcs = 2;
        int trolls = 2 + (_level - 1);

        // Spawn Goblins
        for (int i = 0; i < goblins; i++)
        {
            var g = new Goblin(RandomFloor()); //Picks a random Vector2 position from _floor
            g.AttackPower += statIncrease; //_floor contains all walkable tiles
            enemies.Add(g);
        }

        // Spawn Orcs
        for (int i = 0; i < orcs; i++)
        {
            var o = new Orc(RandomFloor());
            o.AttackPower += statIncrease;
            enemies.Add(o);
        }

        // Spawn Trolls (scales in both count and difficulty)
        for (int i = 0; i < trolls; i++)
        {
            var t = new Troll(RandomFloor());
            t.AttackPower += statIncrease;
            enemies.Add(t);
        }

        return enemies;
    }

    /// <summary>
    /// Spawns items in the dungeon including weapons, armor, potions, and gold.
    /// 
    /// Item generation rules:
    /// - 4 weapons (one of each type)
    /// - 2 random potion types
    /// - 1 special healing potion
    /// - 3 armor pieces (light, medium, heavy)
    /// - 3 gold pickups (random amounts 2–10)
    /// </summary>
    public override List<Item> SpawnItems()
    {
        var items = new List<Item>();

        // Weapons (all types guaranteed per level)
        items.Add(new Weapon(RandomFloor(), WeaponType.Dagger, _level));
        items.Add(new Weapon(RandomFloor(), WeaponType.Sword, _level));
        items.Add(new Weapon(RandomFloor(), WeaponType.Axe, _level));
        items.Add(new Weapon(RandomFloor(), WeaponType.Bow, _level));

        // Random potion selection (2 out of 3 types)
        var potionTypes = Enum.GetValues(typeof(PotionType)) //This gets all values in the enum: PotionType.Healing --> returns array of object
                              .Cast<PotionType>()//we need to convert to IEnumerable [Healing, Power, Guard] --> Treat these values as PotionType -->Ienumerable can be put in loop
                              .OrderBy(_ => _rng.Next())
                              .Take(2);//takes first 2 items

        foreach (var type in potionTypes)
            items.Add(new Potion(RandomFloor(), type));

        // Rare full-heal potion
        items.Add(new SpecialPotion(RandomFloor()));

        // Armor set (one of each type per level)
        items.Add(new Armor(RandomFloor(), ArmorType.Light, _level));
        items.Add(new Armor(RandomFloor(), ArmorType.Medium, _level));
        items.Add(new Armor(RandomFloor(), ArmorType.Heavy, _level));

        // Gold pickups (random value between 2 and 10)
        for (int i = 0; i < 3; i++)
            items.Add(new Gold(RandomFloor(), _rng.Next(2, 11)));

        return items;
    }

    /// <summary>
    /// Spawns the level exit tile.
    /// Ensures the exit does not overlap with any item or enemy position.
    /// </summary>
    public override Vector2 SpawnExit(List<Item> items, List<Enemy> enemies)
    {
        Vector2 pos; //store exit location

        // Keep generating positions until a free tile is found
        do
        {
            pos = RandomFloor(); //picks a random walkable tile
        }
        while (items.Any(i => i.Pos == pos) || //reject if Any item is already there
               enemies.Any(e => e.Pos == pos)); //reject enemies.Any(e => e.Pos == pos));

        return pos;
    }
}