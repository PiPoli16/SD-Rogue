using RogueLib.Dungeon;
using RogueLib.Enemies;
using RogueLib.Engine;
using RogueLib.items;
using RogueLib.Utilities;
using SandBox01.Levels;
using System;
using System.Collections.Generic;
using System.Linq;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

namespace RlGameNS;

public class Level : Scene
{
    private string _map;
    private int _levelNumber;

    private TileSet _floor = new();
    private TileSet _walkables = new();
    private TileSet _exit = new();

    private List<Item> _items = new();
    private List<Enemy> _enemies = new();

    public Level(Player player, string map, Game game, int levelNumber)
    {
        _player = player;
        _game = game;

        _map = map;
        _levelNumber = levelNumber;

        _player.Pos = new Vector2(4, 12);

        initMapTileSets(map);
        registerCommandsWithScene();

        spawnItems();
        spawnEnemies();
    }

    // =========================================================
    // UPDATE
    // =========================================================
    public override void Update()
    {
        if (_player == null) return;

        // ---------------- EXIT ----------------
        if (_exit.Contains(_player.Pos))
        {
            string nextMap = (_levelNumber % 2 == 1)
                ? MyGame.map2
                : MyGame.map1;

            var next = new Level(_player, nextMap, _game!, _levelNumber + 1);
            _game!.LoadLevel(next);
            return;
        }

        // ---------------- ITEM PICKUP ----------------
        var itemsHere = _items.Where(i => i.Pos == _player.Pos).ToList();

        foreach (var item in itemsHere)
        {
            item.Apply(_player);
            _items.Remove(item);
        }

        // ---------------- ENEMIES ----------------
        foreach (var e in _enemies.ToList())
            e.Update(_player, _walkables);

        _player.Update();
    }

    // =========================================================
    // DRAW (SAFE UI LAYOUT)
    // =========================================================
    public override void Draw(IRenderWindow disp)
    {
        // ---------------- SAFE MAP AREA ----------------
        // We reserve bottom 3 rows for UI (22–24)

        int maxMapHeight = 22;

        var lines = _map.Split('\n');

        for (int y = 0; y < Math.Min(lines.Length, maxMapHeight); y++)
        {
            disp.Draw(lines[y], new Vector2(0, y), ConsoleColor.Gray);
        }

        // ---------------- ENTITIES ----------------
        foreach (var item in _items)
            item.Draw(disp);

        foreach (var e in _enemies)
            e.Draw(disp);

        foreach (var p in _exit)
            disp.Draw('X', p, ConsoleColor.Cyan);

        _player.Draw(disp);

        // ---------------- UI (NOW SAFE FOREVER) ----------------
        disp.Draw(_player.HUD, new Vector2(0, 22), ConsoleColor.Green);
        disp.Draw(_player.Message, new Vector2(0, 23), ConsoleColor.Yellow);
    }
    // =========================================================
    // INPUT
    // =========================================================
    public override void DoCommand(Command command)
    {
        if (command.Name == "up") Move(Vector2.N);
        else if (command.Name == "down") Move(Vector2.S);
        else if (command.Name == "left") Move(Vector2.W);
        else if (command.Name == "right") Move(Vector2.E);
    }

    private void Move(Vector2 dir)
    {
        var next = _player.Pos + dir;

        var enemy = _enemies.FirstOrDefault(e => e.Pos == next);

        if (enemy != null)
        {
            enemy.Hp -= _player.Strength;

            if (enemy.Hp <= 0)
            {
                _enemies.Remove(enemy);
                _player.SetMessage($"You hit the {enemy.Name} for {_player.Strength} — {enemy.Name} died!");
            }
            else
            {
                _player.SetMessage($"You hit the {enemy.Name} for {_player.Strength}");
            }

            return;
        }

        if (_walkables.Contains(next))
            _player.Pos = next;
    }

    // =========================================================
    // MAP PARSING
    // =========================================================
    private void initMapTileSets(string map)
    {
        foreach (var (c, p) in Vector2.Parse(map))
        {
            if (c == '.')
            {
                _floor.Add(p);
                _walkables.Add(p);
            }

            if (c == '+' || c == '#')
                _walkables.Add(p);

            if (c == 'X')
            {
                _exit.Add(p);
                _walkables.Add(p);
            }
        }
    }

    // =========================================================
    // ITEMS (NO OVERLAP)
    // =========================================================
    private void spawnItems()
    {
        var rng = new Random();

        for (int i = 0; i < 6; i++)
        {
            Vector2 pos;

            do
            {
                pos = _floor.ElementAt(rng.Next(_floor.Count));
            }
            while (_items.Any(x => x.Pos == pos));

            int roll = rng.Next(4);

            if (roll == 0)
                _items.Add(new Potion(pos));
            else if (roll == 1)
                _items.Add(new Weapon(pos, 2));
            else if (roll == 2)
                _items.Add(new Armor(pos));
            else
                _items.Add(new Gold(pos, rng.Next(2, 11)));
        }
    }

    // =========================================================
    // ENEMIES (NO OVERLAP)
    // =========================================================
    private void spawnEnemies()
    {
        var rng = new Random();
        int count = 5 + _levelNumber;

        for (int i = 0; i < count; i++)
        {
            Vector2 pos;

            do
            {
                pos = _floor.ElementAt(rng.Next(_floor.Count));
            }
            while (_enemies.Any(e => e.Pos == pos) || _items.Any(i => i.Pos == pos));

            int roll = rng.Next(3);

            if (roll == 0) _enemies.Add(new Goblin(pos));
            else if (roll == 1) _enemies.Add(new Orc(pos));
            else _enemies.Add(new Troll(pos));
        }
    }

    // =========================================================
    // COMMANDS
    // =========================================================
    private void registerCommandsWithScene()
    {
        RegisterCommand(ConsoleKey.UpArrow, "up");
        RegisterCommand(ConsoleKey.DownArrow, "down");
        RegisterCommand(ConsoleKey.LeftArrow, "left");
        RegisterCommand(ConsoleKey.RightArrow, "right");

        RegisterCommand(ConsoleKey.W, "up");
        RegisterCommand(ConsoleKey.S, "down");
        RegisterCommand(ConsoleKey.A, "left");
        RegisterCommand(ConsoleKey.D, "right");
    }
}