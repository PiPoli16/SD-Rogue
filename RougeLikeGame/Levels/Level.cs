using RogueLib;
using RogueLib.Dungeon;
using RogueLib.Enemies;
using RogueLib.Engine;
using RogueLib.items;
using RogueLib.Message;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SandBox01.Levels;

public class Level : Scene
{
    private string _map;
    private int _levelNumber;

    private HashSet<Vector2> _floor = new();
    private HashSet<Vector2> _walkables = new();
    private HashSet<Vector2> _exit = new();

    private List<Item> _items = new();
    private List<Enemy> _enemies = new();

    private List<Message> _messages = new();

    private Random rng = new Random();

    public Level(Player player, string map, Game game, int levelNumber)
    {
        _player = player;
        _game = game;
        _map = map;
        _levelNumber = levelNumber;

        // ================= PLAYER SCALING =================
        int hpBonus = (_levelNumber - 1) * 5;
        _player.MaxHP += hpBonus;
        _player.Heal(hpBonus);

        // Fixed spawn
        _player.Pos = new Vector2(4, 12);

        initMapTileSets(map);
        registerCommandsWithScene();

        // ================= SPAWNER =================
        var spawner = new DefaultDungeonSpawner(_floor, _levelNumber);

        _enemies = spawner.SpawnEnemies();
        _items = spawner.SpawnItems();

        _exit.Add(spawner.SpawnExit(_items, _enemies));
        _walkables.Add(_exit.First());
    }

    // ================= UPDATE =================
    public override void Update()
    {
        if (_player == null) return;

        // update messages
        foreach (var m in _messages)
            m.Update();

        _messages.RemoveAll(m => !m.IsAlive);

        // ================= EXIT =================
        if (_exit.Contains(_player.Pos))
        {
            _game!.LoadLevel(new Level(_player, MyGame.map1, _game!, _levelNumber + 1));
            return;
        }

        // ================= ITEM PICKUP =================
        foreach (var item in _items.Where(i => i.Pos == _player.Pos).ToList())
        {
            _player.AddItem(item);
            AddMessage(GetItemPickupMessage(item), ConsoleColor.Yellow);
            _items.Remove(item);
        }

        // ================= ENEMY UPDATE =================
        foreach (var e in _enemies.ToList())
            e.Update(_player, _walkables);

        // ================= ENEMY ATTACK =================
        foreach (var e in _enemies)
        {
            e.TickCooldown();

            if (e.Pos == _player.Pos && e.CanAttack())
            {
                _player.TakeDamage(e.AttackPower, e.Name);
                AddMessage($"-{e.AttackPower} HP from {e.Name}", ConsoleColor.DarkRed);

                e.ResetAttackCooldown(10);
            }
        }

        // ================= AUTO POTION =================
        if (_player.TryAutoPotion())
        {
            AddMessage("Auto-used Potion!", ConsoleColor.Magenta);
        }

        _player.Update();

        // ================= DEATH =================
        if (_player.HP <= 0)
        {
            Console.Clear();
            Console.WriteLine(DungeonConfig.RIP);

            while (true)
            {
                Console.WriteLine("\nRestart? (Y/N)");
                char input = char.ToLower(Console.ReadKey(true).KeyChar);

                if (input == 'y')
                {
                    new MyGame().run();
                    return;
                }
                else if (input == 'n')
                {
                    Environment.Exit(0);
                }
            }
        }
    }

    // ================= DRAW =================
    public override void Draw(IRenderWindow disp)
    {
        string[] lines = _map.Split('\n');

        for (int y = 0; y < lines.Length; y++)
            disp.Draw(lines[y], new Vector2(0, y), ConsoleColor.Gray);

        foreach (var i in _items)
            i.Draw(disp);

        foreach (var e in _enemies)
            e.Draw(disp);

        foreach (var p in _exit)
            disp.Draw('X', p, ConsoleColor.Cyan);

        _player.Draw(disp);

        drawRightPanel(disp);

        int hudY = DungeonConfig.height - 2;
        int msgY = DungeonConfig.height - 1;

        disp.Draw(_player.HUD, new Vector2(0, hudY), ConsoleColor.Green);

        // clear message line
        disp.Draw(new string(' ', 70), new Vector2(0, msgY), ConsoleColor.Black);

        if (_messages.Count > 0)
        {
            var last = _messages[^1];
            disp.Draw(last.Text, new Vector2(0, msgY), last.Color);
        }
    }

    // ================= INPUT =================
    public override void DoCommand(Command command)
    {
        Vector2 dir = command.Name switch
        {
            "up" => Vector2.N,
            "down" => Vector2.S,
            "left" => Vector2.W,
            "right" => Vector2.E,
            _ => Vector2.Zero
        };

        Move(dir);
    }

    // ================= MOVE =================
    private void Move(Vector2 dir)
    {
        var next = _player.Pos + dir;

        if (!_walkables.Contains(next))
            return;

        var enemy = _enemies.FirstOrDefault(e => e.Pos == next);

        if (enemy != null)
        {
            int dmg = _player.Strength;
            enemy.HP -= dmg;

            AddMessage($"Hit {enemy.Name} for {dmg}", ConsoleColor.Red);

            if (enemy.HP <= 0)
            {
                _enemies.Remove(enemy);
                _player.AddGold(enemy.GoldDrop);
                _player.AddStrength(1);

                AddMessage($"{enemy.Name} defeated!", ConsoleColor.Green);
            }
            else
            {
                _player.TakeDamage(enemy.AttackPower, enemy.Name);
            }
        }
        else
        {
            _player.Pos = next;
        }
    }

    // ================= MAP =================
    private void initMapTileSets(string map)
    {
        foreach (var (c, p) in Vector2.Parse(map))
        {
            if (c == '.')
                _floor.Add(p);

            if (c == '.' || c == '+' || c == '#')
                _walkables.Add(p);
        }
    }

    // ================= MESSAGES =================
    private void AddMessage(string text, ConsoleColor color)
    {
        _messages.Add(new Message(text, color));

        if (_messages.Count > 3)
            _messages.RemoveAt(0);
    }

    private string GetItemPickupMessage(Item item)
    {
        return item switch
        {
            Weapon w => $"Picked up {w.Type} (+{w.Power} STR)",

            Armor a => $"Picked up {a.Type} Armor (+{a.DefenseValue} DEF)",

            Potion p =>
                $"Picked up {p.Type} Potion (+{p.Amount} {(p.Type == PotionType.Healing ? "HP" : p.Type == PotionType.Power ? "STR" : "DEF")})",

            SpecialPotion => "Picked up Special Potion (Full Heal)",

            Gold g => $"+{g.Amount} Gold",

            _ => "Picked up Gold!"
        };
    }

    // ================= RIGHT PANEL =================
    private void drawRightPanel(IRenderWindow disp)
    {
        int x = 60;
        int statIncrease = (_levelNumber - 1) * 10;

        disp.Draw("ENEMY STATS", new Vector2(x, 1), ConsoleColor.Yellow);

        disp.Draw("Goblin", new Vector2(x, 3), ConsoleColor.Green);
        disp.Draw($"HP: {5 + statIncrease}", new Vector2(x, 4), ConsoleColor.Gray);
        disp.Draw($"ATK: {5 + statIncrease}", new Vector2(x, 5), ConsoleColor.Gray);

        disp.Draw("Orc", new Vector2(x, 7), ConsoleColor.DarkYellow);
        disp.Draw($"HP: {10 + statIncrease}", new Vector2(x, 8), ConsoleColor.Gray);
        disp.Draw($"ATK: {8 + statIncrease}", new Vector2(x, 9), ConsoleColor.Gray);

        disp.Draw("Troll", new Vector2(x, 11), ConsoleColor.Red);
        disp.Draw($"HP: {20 + statIncrease}", new Vector2(x, 12), ConsoleColor.Gray);
        disp.Draw($"ATK: {15 + statIncrease}", new Vector2(x, 13), ConsoleColor.Gray);

        disp.Draw($"Level: {_levelNumber}", new Vector2(x, 15), ConsoleColor.Cyan);
    }

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