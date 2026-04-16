using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

public class Armor : Item
{
    public ArmorType Type { get; }
    public int DefenseValue { get; }

    public Armor(Vector2 pos, ArmorType type, int level)
        : base('[', pos)
    {
        Type = type;

        DefenseValue = type switch
        {
            ArmorType.Light => 1,
            ArmorType.Medium => 2,
            ArmorType.Heavy => 3,
            _ => 0
        };

        // optional scaling per level
        DefenseValue += level - 1;
    }

    public override void Apply(Player player)
    {
        player.AddArmor(DefenseValue);
    }

    public override void Draw(IRenderWindow disp)
        => disp.Draw(Glyph, Pos, ConsoleColor.Cyan);
}
