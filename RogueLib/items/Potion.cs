using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

public class Potion : Item
{
    public PotionType Type { get; }

    public int Amount =>
        Type switch
        {
            PotionType.Healing => 2,
            PotionType.Power => 2,
            PotionType.Guard => 2,
            _ => 0
        };

    public Potion(Vector2 pos, PotionType type)
        : base('!', pos)
    {
        Type = type;
    }

    public override void Apply(Player player)
    {
        switch (Type)
        {
            case PotionType.Healing:
                player.Heal(Amount);
                break;

            case PotionType.Power:
                player.AddStrength(Amount);
                break;

            case PotionType.Guard:
                player.AddArmor(Amount);
                break;
        }
    }

    public override void Draw(IRenderWindow disp)
        => disp.Draw('!', Pos, ConsoleColor.Magenta);
}