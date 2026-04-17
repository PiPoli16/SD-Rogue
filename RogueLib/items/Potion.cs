using RogueLib.Dungeon;   // Gives access to base Item class
using RogueLib.Utilities; // Gives access to Vector2 (position)

namespace RogueLib.items; // Namespace for item-related classes

/// <summary>
/// Potion item that provides a stat or health boost.
/// </summary>
public class Potion : Item // Inherits from Item
{
    // Stores the type of potion (Healing, Power, Guard)
    public PotionType Type { get; }

    // Property that returns the effect amount based on potion type
    public int Amount =>
        Type switch
        {
            PotionType.Healing => 2, // restores HP
            PotionType.Power => 2,   // increases strength
            PotionType.Guard => 2,   // increases defense
            _ => 0                   // fallback (should not happen)
        };

    // Constructor: creates a potion at a position with a specific type
    public Potion(Vector2 pos, PotionType type)
        : base('!', pos) // Calls Item constructor → sets glyph '!' and position
    {
        Type = type; // Save potion type
    }

    // Defines what happens when the player picks up the potion
    public override void Apply(Player player)
    {
        switch (Type) // Decide effect based on potion type
        {
            case PotionType.Healing:
                player.Heal(Amount); // Restore HP
                break;

            case PotionType.Power:
                player.AddStrength(Amount); // Increase strength
                break;

            case PotionType.Guard:
                player.AddArmor(Amount); // Increase defense
                break;
        }
    }

    // Defines how the potion is drawn on the screen
    public override void Draw(IRenderWindow disp)
        => disp.Draw('!', Pos, ConsoleColor.Magenta);
    // Draw potion at its position with magenta color
}