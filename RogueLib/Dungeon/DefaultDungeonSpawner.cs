using RogueLib.items;
using RogueLib.Enemies;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.Dungeon;

public class DefaultDungeonSpawner : DungeonSpawner
{
    public DefaultDungeonSpawner(HashSet<Vector2> floor, int level)
        : base(floor, level) { }

    public override List<Enemy> SpawnEnemies()
    {
        var enemies = new List<Enemy>();

        int statIncrease = (_level - 1) * 10;

        int goblins = 2;
        int orcs = 2;
        int trolls = 2 + (_level - 1);

        for (int i = 0; i < goblins; i++)
        {
            var g = new Goblin(RandomFloor());
            g.AttackPower += statIncrease;
            enemies.Add(g);
        }

        for (int i = 0; i < orcs; i++)
        {
            var o = new Orc(RandomFloor());
            o.AttackPower += statIncrease;
            enemies.Add(o);
        }

        for (int i = 0; i < trolls; i++)
        {
            var t = new Troll(RandomFloor());
            t.AttackPower += statIncrease;
            enemies.Add(t);
        }

        return enemies;
    }

    public override List<Item> SpawnItems()
    {
        var items = new List<Item>();

        items.Add(new Weapon(RandomFloor(), WeaponType.Dagger, _level));
        items.Add(new Weapon(RandomFloor(), WeaponType.Sword, _level));
        items.Add(new Weapon(RandomFloor(), WeaponType.Axe, _level));
        items.Add(new Weapon(RandomFloor(), WeaponType.Bow, _level));

        var potionTypes = Enum.GetValues(typeof(PotionType))
                              .Cast<PotionType>()
                              .OrderBy(_ => _rng.Next())
                              .Take(2);

        foreach (var type in potionTypes)
            items.Add(new Potion(RandomFloor(), type));

        items.Add(new SpecialPotion(RandomFloor()));

        items.Add(new Armor(RandomFloor(), ArmorType.Light, _level));
        items.Add(new Armor(RandomFloor(), ArmorType.Medium, _level));
        items.Add(new Armor(RandomFloor(), ArmorType.Heavy, _level));

        for (int i = 0; i < 3; i++)
            items.Add(new Gold(RandomFloor(), _rng.Next(2, 11)));

        return items;
    }

    public override Vector2 SpawnExit(List<Item> items, List<Enemy> enemies)
    {
        Vector2 pos;

        do
        {
            pos = RandomFloor();
        }
        while (items.Any(i => i.Pos == pos) ||
               enemies.Any(e => e.Pos == pos));

        return pos;
    }
}