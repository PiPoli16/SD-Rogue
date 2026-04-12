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

    // ---------------- CONSTRUCTOR ----------------
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

    // ---------------- UPDATE ----------------
    public override void Update()
    {
        if (_player == null) return;

        // ---------------- EXIT LEVEL ----------------
        if (_exit.Contains(_player.Pos))
        {
            var next = new Level(_player, _map, _game!, _levelNumber + 1);
            _game!.LoadLevel(next);
            return;
        }

        // ---------------- ITEM PICKUP ----------------
        var item = _items.FirstOrDefault(i => i.Pos == _player.Pos);
        if (item != null)
        {
            item.Apply(_player);     // ✅ important for Gold, Potion, etc.
            _items.Remove(item);
        }

        // ---------------- ENEMIES UPDATE ----------------
        foreach (var e in _enemies.ToList())
        {
            e.Update(_player, _walkables);   // ✅ FIXED: Player passed
        }

        // ---------------- PLAYER UPDATE ----------------
        _player.Update();
    }

    // ---------------- DRAW ----------------
    public override void Draw(IRenderWindow disp)
    {
        // ---------------- MAP ----------------
        disp.Draw(_map, ConsoleColor.Gray);

        // ---------------- ITEMS ----------------
        foreach (var item in _items)
            item.Draw(disp);

        // ---------------- ENEMIES ----------------
        foreach (var e in _enemies)
            e.Draw(disp);

        // ---------------- EXIT ----------------
        foreach (var p in _exit)
            disp.Draw('>', p, ConsoleColor.Cyan);

        // ---------------- PLAYER ----------------
        _player.Draw(disp);

        // ---------------- HUD ----------------
        disp.Draw(_player.HUD, new Vector2(0, 23), ConsoleColor.Green);

        // ---------------- CURRENT MESSAGE ----------------
        disp.Draw(_player.Message, new Vector2(0, 24), ConsoleColor.Yellow);

        // ---------------- COMBAT LOG ----------------
        int y = 25;

        foreach (var msg in _player.Log.TakeLast(5))
        {
            disp.Draw(msg, new Vector2(0, y), ConsoleColor.DarkGray);
            y++;
        }
    }

    // ---------------- INPUT ----------------
    public override void DoCommand(Command command)
    {
        if (command.Name == "up") Move(Vector2.N);
        else if (command.Name == "down") Move(Vector2.S);
        else if (command.Name == "left") Move(Vector2.W);
        else if (command.Name == "right") Move(Vector2.E);
    }

    // ---------------- MOVEMENT ----------------
    private void Move(Vector2 dir)
    {
        var next = _player.Pos + dir;

        // ENEMY COLLISION
        var enemy = _enemies.FirstOrDefault(e => e.Pos == next);

        if (enemy != null)
        {
            enemy.Hp -= _player.Strength;

            _player.SetMessage($"Hit {enemy.Glyph} for {_player.Strength}");

            if (enemy.Hp <= 0)
            {
                _enemies.Remove(enemy);
                _player.SetMessage($"{enemy.Glyph} died!");
            }

            return;
        }

        // WALK
        if (_walkables.Contains(next))
            _player.Pos = next;
    }

    // ---------------- MAP ----------------
    private void initMapTileSets(string map)
    {
        foreach (var (c, p) in Vector2.Parse(map))
        {
            if (c == '.')
            {
                _floor.Add(p);
                _walkables.Add(p);
            }

            if (c == '+')
                _walkables.Add(p);

            if (c == '#')
                _walkables.Add(p);

            if (c == '>')
            {
                _exit.Add(p);
                _walkables.Add(p);
            }
        }
    }

    // ---------------- ITEMS ----------------
    private void spawnItems()
    {
        var rng = new Random();
        int count = 6;

        for (int i = 0; i < count; i++)
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));

            int roll = rng.Next(4);

            if (roll == 0)
                _items.Add(new Potion(pos));
            else if (roll == 1)
                _items.Add(new Weapon(pos, 2));
            else if (roll == 2)
                _items.Add(new Armor(pos));
            else
                _items.Add(new Gold(pos, rng.Next(5, 21)));
        }
    }

    // ---------------- ENEMIES ----------------
    private void spawnEnemies()
    {
        var rng = new Random();
        int count = 5 + _levelNumber;

        for (int i = 0; i < count; i++)
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));

            int roll = rng.Next(3);

            if (roll == 0) _enemies.Add(new Goblin(pos));
            else if (roll == 1) _enemies.Add(new Orc(pos));
            else _enemies.Add(new Troll(pos));
        }
    }

    // ---------------- COMMANDS ----------------
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