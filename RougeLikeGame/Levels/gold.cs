using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace SandBox01.Levels;

public class Gold : Item
{
    public int Amount { get; init; }

    public Gold(Vector2 pos, int amt) : base('*', pos)
    {
        Amount = amt;
    }

    public override void Apply(Player player)
    {
        player.AddGold(Amount);
        player.SetMessage($"💰 Gold +{Amount}");
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Yellow);
    }
}