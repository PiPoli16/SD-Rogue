using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace SandBox01.Levels;

public class Weapon : Item
{
    public int Damage { get; }

    public Weapon(Vector2 pos, int dmg)
        : base(')', pos)
    {
        Damage = dmg;
    }

    public override void Apply(Player player)
    {
        player.AddStrength(Damage);
        player.SetMessage($"🪓 Picked up an axe! +{Damage} STR gained");
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.White);
    }
}