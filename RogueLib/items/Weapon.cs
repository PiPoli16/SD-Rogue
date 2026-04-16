using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

public class Weapon : Item
{
    public int Power { get; }
    public WeaponType Type { get; }

    public Weapon(Vector2 pos, WeaponType type, int level) : base(')', pos)
    {
        Type = type;

        int basePower = type switch
        {
            WeaponType.Dagger => 1,
            WeaponType.Sword => 3,
            WeaponType.Axe => 5,
            WeaponType.Bow => 1,
            _ => 1
        };

        Power = basePower + (level - 1); // scaling per level
    }

    public override void Apply(Player player)
    {
        player.AddStrength(Power);
    }

    public override void Draw(IRenderWindow disp)
    {
        ConsoleColor color = Type switch
        {
            WeaponType.Dagger => ConsoleColor.Gray,
            WeaponType.Sword => ConsoleColor.White,
            WeaponType.Axe => ConsoleColor.DarkYellow,
            WeaponType.Bow => ConsoleColor.Green,
            _ => ConsoleColor.White
        };

        disp.Draw(Glyph, Pos, color);
    }
}