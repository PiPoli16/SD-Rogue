using RogueLib.Utilities;
using RogueLib.Dungeon;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public abstract class Enemy : IDrawable
{
    public Vector2 Pos;
    public int Hp;
    public char Glyph;

    public abstract string Name { get; }

    protected Enemy(Vector2 pos, char glyph, int hp)
    {
        Pos = pos;
        Glyph = glyph;
        Hp = hp;
    }

    // ---------------- CORE UPDATE ----------------
    public abstract void Update(Player player, TileSet walkables);

    // ---------------- MOVEMENT ----------------
    protected void MoveToward(Vector2 target, TileSet walkables)
    {
        var dir = new Vector2(
            Math.Sign(target.X - Pos.X),
            Math.Sign(target.Y - Pos.Y)
        );

        var newPos = Pos + dir;

        if (walkables.Contains(newPos))
            Pos = newPos;
    }

    // ---------------- ATTACK SYSTEM ----------------
    protected void Attack(Player player, int damage)
    {
        player.TakeDamage(damage, Name);
    }

    // ---------------- DRAW ----------------
    public void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Red);
    }
}