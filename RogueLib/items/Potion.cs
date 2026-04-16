using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

/// <summary>
/// Potion item that provides a temporary stat or health boost.
/// 
/// Potions are consumable items that are applied immediately
/// when picked up by the player (via AddItem + Apply).
/// </summary>
public class Potion : Item
{
    /// <summary>
    /// Type of potion determines its effect:
    /// - Healing: restores HP
    /// - Power: increases strength
    /// - Guard: increases defense
    /// </summary>
    public PotionType Type { get; }

    /// <summary>
    /// Effect strength of the potion.
    /// Currently fixed to 2 for all types.
    /// </summary>
    public int Amount =>
        Type switch
        {
            PotionType.Healing => 2,
            PotionType.Power => 2,
            PotionType.Guard => 2,
            _ => 0
        };

    /// <summary>
    /// Creates a potion item at a given position with a specific type.
    /// </summary>
    public Potion(Vector2 pos, PotionType type)
        : base('!', pos)
    {
        Type = type;
    }

    /// <summary>
    /// Applies the potion effect immediately to the player.
    /// 
    /// This is called when the player picks up the item.
    /// </summary>
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

    /// <summary>
    /// Draws the potion on screen using a magenta color.
    /// </summary>
    public override void Draw(IRenderWindow disp)
        => disp.Draw('!', Pos, ConsoleColor.Magenta);
}