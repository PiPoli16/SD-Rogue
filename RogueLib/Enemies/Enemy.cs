using RogueLib.Dungeon;
using RogueLib.Enemies;
using RogueLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class Enemy : IDrawable
{
    public Vector2 Pos;
    public string Name;
    public char Glyph;
    public int MaxHP;
    public int HP;
    public int AttackPower;
    public int Defense;
    public int Speed;
    public int GoldDrop;

    private int _moveCounter = 0;
    private int _ticksPerMove;

    protected Enemy(Vector2 pos, string name, char glyph, int health, int attack, int defense, int speed, int gold, int ticksPerMove = 5)
    {
        Pos = pos;
        Name = name;
        Glyph = glyph;
        MaxHP = health + defense;
        HP = MaxHP;
        AttackPower = attack;
        Defense = defense;
        Speed = speed;
        GoldDrop = gold;
        _ticksPerMove = ticksPerMove;
    }

    public virtual void Update(Player player, HashSet<Vector2> walkables)
    {
        _moveCounter++;
        if (_moveCounter < _ticksPerMove) return;
        _moveCounter = 0;

        for (int i = 0; i < Speed; i++)
            MoveAI(player, walkables);
    }

    private void MoveAI(Player player, HashSet<Vector2> walkables)
    {
        bool shouldFlee = AttackPower < player.Strength || (this is Troll && HP <= MaxHP / 2);

        Vector2 bestMove = Pos;

        if (shouldFlee)
        {
            // Generate all possible adjacent moves
            List<Vector2> possibleMoves = new List<Vector2>()
            {
                Pos + new Vector2(-1, 0),
                Pos + new Vector2(1, 0),
                Pos + new Vector2(0, -1),
                Pos + new Vector2(0, 1),
                Pos + new Vector2(-1, -1),
                Pos + new Vector2(-1, 1),
                Pos + new Vector2(1, -1),
                Pos + new Vector2(1, 1)
            };

            // Filter only walkable tiles
            var validMoves = possibleMoves.Where(p => walkables.Contains(p)).ToList();
            if (validMoves.Count > 0)
            {
                // Pick the move that **increases distance from player**
                bestMove = validMoves.OrderByDescending(p => Vector2.manhattanDistance(p, player.Pos)).First();
            }
        }
        else
        {
            // Chase player
            Vector2 dir = new Vector2(
                Math.Sign(player.Pos.X - Pos.X),
                Math.Sign(player.Pos.Y - Pos.Y)
            );
            Vector2 newPos = Pos + dir;
            if (walkables.Contains(newPos))
                bestMove = newPos;
        }

        Pos = bestMove;
    }

    public void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Red);
    }
}