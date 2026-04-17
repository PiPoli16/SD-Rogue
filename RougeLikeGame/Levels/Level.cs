// Importing namespaces (libraries) needed for the game
using RogueLib;                 // Core game framework
using RogueLib.Dungeon;        // Dungeon/map-related utilities
using RogueLib.Enemies;        // Enemy classes
using RogueLib.Engine;         // Game engine (Scene, rendering, etc.)
using RogueLib.items;          // Item classes (weapons, potions, etc.)
using RogueLib.Message;        // Message system (UI feedback)
using RogueLib.Utilities;      // Utility classes like Vector2
using System;
using System.Collections.Generic;
using System.Linq;

// Namespace for organizing this Level class
namespace SandBox01.Levels;

// Level class inherits from Scene (meaning it is a playable game scene)
public class Level : Scene
{
    // ================= FIELDS =================

    private string _map;                  // Raw string representation of the map
    private int _levelNumber;             // Current level number (difficulty scaling)

    private HashSet<Vector2> _floor = new();      // Tiles that are floor
    private HashSet<Vector2> _walkables = new();  // Tiles player/enemies can walk on
    private HashSet<Vector2> _exit = new();       // Exit tile(s)

    private List<Item> _items = new();    // All items currently on the map
    private List<Enemy> _enemies = new(); // All enemies currently on the map

    private List<Message> _messages = new(); // Messages displayed to the player

    private Random rng = new Random();    // Random number generator

    // ================= CONSTRUCTOR =================
    // Called when a new level is created
    public Level(Player player, string map, Game game, int levelNumber)
    {
        _player = player;           // Assign player reference
        _game = game;               // Assign game reference
        _map = map;                // Store map string
        _levelNumber = levelNumber; // Store level number

        // ================= PLAYER SCALING =================
        // Increase player HP based on level (difficulty progression)
        int hpBonus = (_levelNumber - 1) * 5;

        _player.MaxHP += hpBonus;   // Increase max HP
        _player.Heal(hpBonus);      // Heal player to match new max

        // Set player's starting position (fixed spawn point)
        _player.Pos = new Vector2(4, 12);

        // Convert map string into usable tile sets
        initMapTileSets(map);

        // Register keyboard controls
        registerCommandsWithScene();

        // ================= SPAWNER =================
        // Creates enemies and items based on level
        var spawner = new DefaultDungeonSpawner(_floor, _levelNumber);

        _enemies = spawner.SpawnEnemies(); // Generate enemies
        _items = spawner.SpawnItems();     // Generate items

        // Spawn exit tile and make it walkable
        _exit.Add(spawner.SpawnExit(_items, _enemies));
        _walkables.Add(_exit.First());
    }

    // ================= UPDATE =================
    // Called every game tick (game loop)
    public override void Update()
    {
        // If player doesn't exist, stop update
        if (_player == null) return;

        // ================= UPDATE MESSAGES =================
        // Update all messages (for timers, fading, etc.)
        foreach (var m in _messages)
            m.Update();

        // Remove messages that are no longer active
        _messages.RemoveAll(m => !m.IsAlive);

        // ================= EXIT =================
        // If player reaches exit, load next level
        if (_exit.Contains(_player.Pos))
        {
            _game!.LoadLevel(new Level(_player, MyGame.map1, _game!, _levelNumber + 1));
            return;
        }

        // ================= ITEM PICKUP =================
        // Check if player is standing on an item
        foreach (var item in _items.Where(i => i.Pos == _player.Pos).ToList())
        {
            _player.AddItem(item); // Add item to inventory

            // Show pickup message
            AddMessage(GetItemPickupMessage(item), ConsoleColor.Yellow);

            _items.Remove(item); // Remove item from map
        }

        // ================= ENEMY UPDATE =================
        // Update each enemy (movement, AI, etc.)
        foreach (var e in _enemies.ToList())
            e.Update(_player, _walkables);

        // ================= ENEMY ATTACK =================
        foreach (var e in _enemies)
        {
            e.TickCooldown(); // Reduce attack cooldown timer

            // If enemy is on same tile and can attack
            if (e.Pos == _player.Pos && e.CanAttack())
            {
                // Deal damage to player
                _player.TakeDamage(e.AttackPower, e.Name);

                // Show damage message
                AddMessage($"-{e.AttackPower} HP from {e.Name}", ConsoleColor.DarkRed);

                // Reset cooldown so enemy cannot attack every frame
                e.ResetAttackCooldown(10);
            }
        }

        // ================= AUTO POTION =================
        // Automatically use potion if needed
        if (_player.TryAutoPotion())
        {
            AddMessage("Auto-used Potion!", ConsoleColor.Magenta);
        }

        // Update player (movement, status, etc.)
        _player.Update();

        // ================= DEATH =================
        // If player HP reaches 0 → game over
        if (_player.HP <= 0)
        {
            Console.Clear();
            Console.WriteLine(DungeonConfig.RIP); // Show RIP screen

            // Infinite loop waiting for user decision
            while (true)
            {
                Console.WriteLine("\nRestart? (Y/N)");
                char input = char.ToLower(Console.ReadKey(true).KeyChar);

                if (input == 'y')
                {
                    new MyGame().run(); // Restart game
                    return;
                }
                else if (input == 'n')
                {
                    Environment.Exit(0); // Exit application
                }
            }
        }
    }

    // ================= DRAW =================
    // Responsible for rendering everything on screen
    public override void Draw(IRenderWindow disp)
    {
        // Split map into lines (rows)
        string[] lines = _map.Split('\n');

        // Draw map background
        for (int y = 0; y < lines.Length; y++)
            disp.Draw(lines[y], new Vector2(0, y), ConsoleColor.Gray);

        // Draw all items
        foreach (var i in _items)
            i.Draw(disp);

        // Draw all enemies
        foreach (var e in _enemies)
            e.Draw(disp);

        // Draw exit tile(s)
        foreach (var p in _exit)
            disp.Draw('X', p, ConsoleColor.Cyan);

        // Draw player
        _player.Draw(disp);

        // Draw right-side stats panel
        drawRightPanel(disp);

        // HUD and message positions
        int hudY = DungeonConfig.height - 2;
        int msgY = DungeonConfig.height - 1;

        // Draw player HUD (HP, gold, etc.)
        disp.Draw(_player.HUD, new Vector2(0, hudY), ConsoleColor.Green);

        // Clear message line before drawing new message
        disp.Draw(new string(' ', 70), new Vector2(0, msgY), ConsoleColor.Black);

        // Draw latest message
        if (_messages.Count > 0)
        {
            var last = _messages[^1];
            disp.Draw(last.Text, new Vector2(0, msgY), last.Color);
        }
    }

    // ================= INPUT =================
    // Handles keyboard input mapped to commands
    public override void DoCommand(Command command)
    {
        // Convert command name into direction vector
        Vector2 dir = command.Name switch
        {
            "up" => Vector2.N,
            "down" => Vector2.S,
            "left" => Vector2.W,
            "right" => Vector2.E,
            _ => Vector2.Zero
        };

        Move(dir); // Move player
    }

    // ================= MOVE =================
    private void Move(Vector2 dir)
    {
        var next = _player.Pos + dir; // Calculate next position

        // If tile is not walkable → stop
        if (!_walkables.Contains(next))
            return;

        // Check if enemy is on next tile
        var enemy = _enemies.FirstOrDefault(e => e.Pos == next);

        if (enemy != null)
        {
            // Player attacks enemy
            int dmg = _player.Strength;
            enemy.HP -= dmg;

            AddMessage($"Hit {enemy.Name} for {dmg}", ConsoleColor.Red);

            if (enemy.HP <= 0)
            {
                // Enemy dies
                _enemies.Remove(enemy);

                _player.AddGold(enemy.GoldDrop);
                _player.AddStrength(1);

                AddMessage($"{enemy.Name} defeated!", ConsoleColor.Green);
            }
            else
            {
                // Enemy counterattacks
                _player.TakeDamage(enemy.AttackPower, enemy.Name);
            }
        }
        else
        {
            // No enemy → move player
            _player.Pos = next;
        }
    }

    // ================= MAP =================
    // Converts map string into usable tile sets
    private void initMapTileSets(string map)
    {
        foreach (var (c, p) in Vector2.Parse(map))
        {
            // '.' = floor
            if (c == '.')
                _floor.Add(p);

            // Walkable tiles
            if (c == '.' || c == '+' || c == '#')
                _walkables.Add(p);
        }
    }

    // ================= MESSAGES =================
    private void AddMessage(string text, ConsoleColor color)
    {
        _messages.Add(new Message(text, color));

        // Keep only last 3 messages
        if (_messages.Count > 3)
            _messages.RemoveAt(0);
    }

    // Generates message depending on item type
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
    // Displays enemy stats and level info
    private void drawRightPanel(IRenderWindow disp)
    {
        int x = 60; // Panel X position
        int statIncrease = (_levelNumber - 1) * 10; // Difficulty scaling

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

    // ================= INPUT REGISTRATION =================
    // Maps keyboard keys to commands
    private void registerCommandsWithScene()
    {
        RegisterCommand(ConsoleKey.UpArrow, "up");
        RegisterCommand(ConsoleKey.DownArrow, "down");
        RegisterCommand(ConsoleKey.LeftArrow, "left");
        RegisterCommand(ConsoleKey.RightArrow, "right");

        // WASD support
        RegisterCommand(ConsoleKey.W, "up");
        RegisterCommand(ConsoleKey.S, "down");
        RegisterCommand(ConsoleKey.A, "left");
        RegisterCommand(ConsoleKey.D, "right");
    }
}