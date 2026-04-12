using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

public class Armor : Item
{
    public Armor(Vector2 pos)
        : base('[', pos)
    { }

    public override void Apply(Player player)
    {
        player.AddArmor(2);
        player.SetMessage("🛡 You equipped armor! +2 DEF gained");
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Cyan);
    }
}