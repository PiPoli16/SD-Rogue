using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

public class Potion : Item
{
    public Potion(Vector2 pos)
        : base('!', pos)
    { }

    public override void Apply(Player player)
    {
        player.Heal(5);
        player.SetMessage("🧪 You drank a potion! +5 HP");
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Magenta);
    }
}