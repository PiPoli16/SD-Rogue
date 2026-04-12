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

            // Check exit
            if (_exit.Contains(_player.Pos))
            {
                string nextMap = (_levelNumber % 2 == 1) ? MyGame.map2 : MyGame.map1;
                _game!.LoadLevel(new Level(_player, nextMap, _game!, _levelNumber + 1));
                return;
            }

            // Pickup items
            var itemsHere = _items.Where(i => i.Pos == _player.Pos).ToList();
            foreach (var item in itemsHere)
            {
                item.Apply(_player);
                _items.Remove(item);
            }

            // Player-enemy collision
            var enemy = _enemies.FirstOrDefault(e => e.Pos == _player.Pos);
            if (enemy != null)
            {
                int playerAttack = _player.Strength + _player.Inventory.Count(i => i is Weapon) * 5;
                if (playerAttack >= enemy.AttackPower)
                {
                    enemy.HP -= playerAttack;
                    _player.SetMessage($"You hit {enemy.Name} for {playerAttack} damage!");
                    if (enemy.HP <= 0)
                    {
                        _enemies.Remove(enemy);
                        _player.AddGold(enemy.GoldDrop);
                        _player.SetMessage($"{enemy.Name} died! +{enemy.GoldDrop} gold");
                        if (enemy is Goblin || enemy is Orc)
                        {
                            _player.AddStrength(1);
                            _player.SetMessage($"+1 STR gained from killing {enemy.Name}");
                        }
                    }
                }
                else
                {
                    _player.TakeDamage(enemy.AttackPower, enemy.Name);
                }
            }

            // ---------------- ENEMIES UPDATE ----------------
            foreach (var e in _enemies.ToList())
                e.Update(_player, _walkables);

            // Auto potion
            _player.TryAutoPotion();

            _player.Update();

            // Player death
            if (_player.HP <= 0)
            {
                Console.Clear();
                Console.WriteLine(DungeonConfig.RIP);
                Console.WriteLine("\nDo you want to start again? (Y/N)");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y)
                {
                    Game game = new MyGame();
                    game.run();
                }
                else
                {
                    Environment.Exit(0);
                }
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
            foreach (var p in _exit) disp.Draw('X', p, ConsoleColor.Cyan);
            _player.Draw(disp);

            int hudY = mapLines.Length;
            disp.Draw(_player.HUD, new Vector2(0, hudY), ConsoleColor.Green);
            disp.Draw(_player.Message, new Vector2(0, hudY + 1), ConsoleColor.Yellow);
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
                int playerAttack = _player.Strength + _player.Inventory.Count(i => i is Weapon) * 5;
                if (playerAttack >= enemy.AttackPower)
                {
                    enemy.HP -= playerAttack;
                    _player.SetMessage($"You hit {enemy.Name} for {playerAttack} damage!");
                    if (enemy.HP <= 0)
                    {
                        _enemies.Remove(enemy);
                        _player.AddGold(enemy.GoldDrop);
                        _player.SetMessage($"{enemy.Name} died! +{enemy.GoldDrop} gold");
                        if (enemy is Goblin || enemy is Orc)
                        {
                            _player.AddStrength(1);
                            _player.SetMessage($"+1 STR gained from killing {enemy.Name}");
                        }
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
                if (c == 'X') { _exit.Add(p); _walkables.Add(p); }
            }
        }

        // ---------------- ITEMS ----------------
        private void spawnItems()
        {
            var rng = new Random();
            int armorCount = (_levelNumber == 1) ? 2 : 1;
            int potionCount = (_levelNumber == 1) ? 2 : 1;
            int specialPotionCount = (_levelNumber == 1) ? 0 : 1;
            int weaponCount = (_levelNumber == 1) ? 2 : 1;

            for (int i = 0; i < armorCount; i++) _items.Add(new Armor(randomFloorPos(rng)));
            for (int i = 0; i < potionCount; i++) _items.Add(new Potion(randomFloorPos(rng)));
            for (int i = 0; i < specialPotionCount; i++) _items.Add(new SpecialPotion(randomFloorPos(rng)));
            for (int i = 0; i < weaponCount; i++) _items.Add(new Weapon(randomFloorPos(rng), 5));

            for (int i = 0; i < 3; i++) _items.Add(new Gold(randomFloorPos(rng), rng.Next(2, 11)));
        }

        // ---------------- ENEMIES ----------------
        private void spawnEnemies()
        {
            var rng = new Random();

            // Dynamic stat scaling: Level 1 = 0, Level 2 = 10, Level 3 = 15, etc.
            int statIncrease = (_levelNumber == 1) ? 0 : (_levelNumber - 1) * 5 + 5;

            int goblinCount = (_levelNumber == 1) ? 2 : 3;
            int orcCount = (_levelNumber == 1) ? 2 : 3;
            int trollCount = (_levelNumber == 1) ? 2 : 3;

            for (int i = 0; i < goblinCount; i++)
            {
                var g = new Goblin(randomFloorPos(rng));
                g.HP += statIncrease; g.MaxHP += statIncrease; g.AttackPower += statIncrease; g.Defense += statIncrease;
                _enemies.Add(g);
            }
            for (int i = 0; i < orcCount; i++)
            {
                var o = new Orc(randomFloorPos(rng));
                o.HP += statIncrease; o.MaxHP += statIncrease; o.AttackPower += statIncrease; o.Defense += statIncrease;
                _enemies.Add(o);
            }
            for (int i = 0; i < trollCount; i++)
            {
                var t = new Troll(randomFloorPos(rng));
                t.HP += statIncrease; t.MaxHP += statIncrease; t.AttackPower += statIncrease; t.Defense += statIncrease;
                _enemies.Add(t);
            }
        }

        private Vector2 randomFloorPos(Random rng)
        {
            Vector2 pos;
            do
            {
                pos = _floor.ElementAt(rng.Next(_floor.Count));
            } while (_items.Any(i => i.Pos == pos) || _enemies.Any(e => e.Pos == pos));
            return pos;
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
}