using RogueLib;
using RogueLib.Dungeon;
using RogueLib.Enemies;
using RogueLib.Engine;
using RogueLib.items;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

namespace RlGameNS
{
    public class Level : Scene
    {
        private string _map;
        private int _levelNumber;
        private TileSet _floor = new();
        private TileSet _walkables = new();
        private TileSet _exit = new();
        private List<Item> _items = new();
        private List<Enemy> _enemies = new();
        private Random rng = new Random();

        public Level(Player player, string map, Game game, int levelNumber)
        {
            _player = player;
            _game = game;
            _map = map;
            _levelNumber = levelNumber;

            // Only increase max HP (no healing)
            if (_levelNumber > 1)
                _player.MaxHP += 5;

            _player.Pos = new Vector2(4, 12);

            initMapTileSets(map);
            registerCommandsWithScene();
            spawnItems();
            spawnEnemies();
            placeRandomExit();
        }

        // ---------------- UPDATE ----------------
        public override void Update()
        {
            if (_player == null) return;

            if (_exit.Contains(_player.Pos))
            {
                _game!.LoadLevel(new Level(_player, MyGame.map1, _game!, _levelNumber + 1));
                return;
            }

            // Pickup items
            var itemsHere = _items.Where(i => i.Pos == _player.Pos).ToList();
            foreach (var item in itemsHere)
            {
                _player.AddItem(item);
                _items.Remove(item);
            }

            // Enemies
            foreach (var e in _enemies.ToList())
                e.Update(_player, _walkables);

            _player.TryAutoPotion();
            _player.Update();

            if (_player.HP <= 0)
            {
                Console.Clear();
                Console.WriteLine(DungeonConfig.RIP);
                Console.WriteLine("\nRestart? (Y/N)");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y)
                    new MyGame().run();
                else
                    Environment.Exit(0);
            }
        }

        // ---------------- DRAW ----------------
        public override void Draw(IRenderWindow disp)
        {
            string[] mapLines = _map.Split('\n');

            for (int y = 0; y < mapLines.Length; y++)
                disp.Draw(mapLines[y], new Vector2(0, y), ConsoleColor.Gray);

            foreach (var item in _items) item.Draw(disp);
            foreach (var e in _enemies) e.Draw(disp);

            foreach (var p in _exit)
                disp.Draw('X', p, ConsoleColor.Cyan);

            _player.Draw(disp);

            disp.Draw(_player.HUD, new Vector2(0, mapLines.Length), ConsoleColor.Green);
        }

        // ---------------- INPUT ----------------
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

        private void Move(Vector2 dir)
        {
            var next = _player.Pos + dir;
            if (!_walkables.Contains(next)) return;

            var enemy = _enemies.FirstOrDefault(e => e.Pos == next);

            if (enemy != null)
            {
                // ✅ FIXED: No more double counting
                int playerAttack = _player.Strength;

                _player.AddLog($"You hit {enemy.Name} for {playerAttack}");

                if (playerAttack >= enemy.AttackPower)
                {
                    enemy.HP -= playerAttack;

                    if (enemy.HP <= 0)
                    {
                        _enemies.Remove(enemy);
                        _player.AddGold(enemy.GoldDrop);

                        _player.AddLog($"{enemy.Name} died");

                        // ✅ ALL enemies give +1 STR
                        _player.AddStrength(1);
                    }
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

            _player.TryAutoPotion();
        }

        // ---------------- MAP ----------------
        private void initMapTileSets(string map)
        {
            foreach (var (c, p) in Vector2.Parse(map))
            {
                if (c == '.') { _floor.Add(p); _walkables.Add(p); }
                if (c == '+' || c == '#') _walkables.Add(p);
            }
        }

        private void placeRandomExit()
        {
            var pos = _floor.ElementAt(rng.Next(_floor.Count));
            _exit.Add(pos);
            _walkables.Add(pos);
        }

        // ---------------- ITEMS ----------------
        private void spawnItems()
        {
            if (_levelNumber == 1)
            {
                for (int i = 0; i < 2; i++) _items.Add(new Weapon(randomFloorPos(), 5));
                for (int i = 0; i < 2; i++) _items.Add(new Potion(randomFloorPos()));
                for (int i = 0; i < 2; i++) _items.Add(new Armor(randomFloorPos()));
            }
            else
            {
                _items.Add(new Weapon(randomFloorPos(), 5));
                _items.Add(new Potion(randomFloorPos()));
                _items.Add(new SpecialPotion(randomFloorPos()));
                _items.Add(new Armor(randomFloorPos()));
            }

            for (int i = 0; i < 3 + (_levelNumber - 1); i++)
                _items.Add(new Gold(randomFloorPos(), rng.Next(2, 11)));
        }

        // ---------------- ENEMIES ----------------
        private void spawnEnemies()
        {
            int statIncrease = (_levelNumber == 1) ? 0 : (_levelNumber - 1) * 5 + 5;

            for (int i = 0; i < 2 + (_levelNumber > 1 ? 1 : 0); i++)
            {
                var g = new Goblin(randomFloorPos());
                g.AttackPower += statIncrease;
                _enemies.Add(g);

                var o = new Orc(randomFloorPos());
                o.AttackPower += statIncrease;
                _enemies.Add(o);

                var t = new Troll(randomFloorPos());
                t.AttackPower += statIncrease;
                _enemies.Add(t);
            }
        }

        private Vector2 randomFloorPos()
        {
            return _floor.ElementAt(rng.Next(_floor.Count));
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
}