using RogueLib.Engine;
using RogueLib.Utilities;
using RogueLib.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueLib.Dungeon;

public class BaseLevel : Scene
{
    protected string? _map;
    protected Vector2 _exitPos;
    protected List<Enemy> _enemies = new();


    public BaseLevel(Player player, Game game, string map)
    {
        _player = player;
        _game = game;
        _map = map;

        // exit
        _exitPos = new Vector2(70, 20);

        // ⭐ For test   
        _enemies.Add(new Goblin(new Vector2(10, 10)));
        _enemies.Add(new Orc(new Vector2(15, 12)));
        _enemies.Add(new Troll(new Vector2(20, 15)));
    }

    // ================= COMMAND =================
    public override void DoCommand(Command command)
    {
        switch (command.Name)
        {
            case "UP":
                _player.Pos += Vector2.N;
                break;
            case "DOWN":
                _player.Pos += Vector2.S;
                break;
            case "LEFT":
                _player.Pos += Vector2.W;
                break;
            case "RIGHT":
                _player.Pos += Vector2.E;
                break;
        }
    }

    // ================= DRAW =================
    public override void Draw(IRenderWindow disp)
    {
        // Map
        if (_map != null)
            disp.Draw(_map, ConsoleColor.DarkGray);

        // Exit
        disp.Draw('X', _exitPos, ConsoleColor.Green);

        // enemy
        foreach (var enemy in _enemies)
            enemy.Draw(disp);

        // palyer
        _player.Draw(disp);

        //  HUD
        disp.Draw(_player.HUD, new Vector2(0, 0), ConsoleColor.White);

        //  Log（Notification）
        int y = 1;
        foreach (var log in _player.Logs)
        {
            disp.Draw(log, new Vector2(0, y), ConsoleColor.Yellow);
            y++;
        }
    }

    // ================= UPDATE =================
    public override void Update()
    {
        // ⭐ Exit 
        if (_player.Pos == _exitPos)
        {
            OnLevelComplete();
            _levelActive = false;
            return;
        }

        if (_player.IsDead)
        {
            OnPlayerDeath();
            _levelActive = false;
            return;
        }
        // ⭐ Add Potion
        _player.TryAutoPotion();

        // ⭐ Enemy move
        var walkables = GetWalkableTiles();

        foreach (var enemy in _enemies)
            enemy.Update(_player, walkables);

        // ⭐ Combat
        foreach (var enemy in _enemies.ToList())
        {
            if (enemy.Pos == _player.Pos)
            {
                int playerAttack = _player.Strength;

                // attack
                enemy.HP -= playerAttack;
                _player.AddLog($"Hit {enemy.Name} for {playerAttack}");

                // 💀 dead
                if (enemy.HP <= 0)
                {
                    _player.AddKill();
                    _player.AddGold(enemy.GoldDrop);

                    // ⭐ Rule：Goblin / Orc +1 STR
                    if (enemy is Goblin || enemy is Orc)
                        _player.AddStrength(1);

                    _player.AddLog($"{enemy.Name} killed!");
                    _enemies.Remove(enemy);
                }
                else
                {
                    // 👹 damage enemy
                    int dmg = enemy.AttackPower;
                    _player.TakeDamage(dmg, enemy.Name);
                }
            }
        }
    }
    private void OnPlayerDeath()
    {
        _player?.AddLog("You died...");

        _game?.LoadEndScene(_player!);
    }


    // ================= EXIT =================
    private void OnLevelComplete()
    {
        _player?.AddLog("Level Complete!");

        Console.Clear();
        Console.WriteLine("=== LEVEL COMPLETE ===");
        Console.WriteLine(_player?.HUD);
        Console.WriteLine();
        Console.WriteLine($"Kills: {_player?.Kills}");
        Console.WriteLine($"Potions Used: {_player?.PotionsUsed}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();

        _game?.LoadNextLevel();
    }

    // ================= WALKABLE =================
    private HashSet<Vector2> GetWalkableTiles()
    {
        var set = new HashSet<Vector2>();

        if (_map == null) return set;

        foreach (var (ch, pos) in Vector2.Parse(_map))
        {
            if (ch == '.' || ch == '#' || ch == '+' || ch == ' ')
                set.Add(pos);
        }

        return set;
    }
}