using RogueLib.Dungeon;   // Access to base Item class
using RogueLib.Utilities; // Access to Vector2 (position)

namespace RogueLib.items; // Namespace for item-related classes

/// <summary>
/// Armor item that increases the player's defense (shield).
/// </summary>
public class Armor : Item // Inherits from Item
{
    // Stores the type of armor (Light, Medium, Heavy)
    public ArmorType Type { get; }

    // Stores the final defense value after calculation
    public int DefenseValue { get; }

    // Constructor: creates armor with position, type, and level scaling
    public Armor(Vector2 pos, ArmorType type, int level)
        : base('[', pos) // Calls Item constructor → sets glyph '[' and position
    {
        Type = type; // Save armor type

        // Determine base defense depending on armor type
        DefenseValue = type switch
        {
            ArmorType.Light => 1,   // lowest defense
            ArmorType.Medium => 2,  // medium defense
            ArmorType.Heavy => 3,   // highest defense
            _ => 0                  // fallback
        };

        // Apply level scaling:
        // Level 1 → +0
        // Level 2 → +1
        // Level 3 → +2
        DefenseValue += level - 1;
    }

    // Defines what happens when player picks up the armor
    public override void Apply(Player player)
    {
        player.AddArmor(DefenseValue); // Increase player's defense (shield)
    }

    // Defines how the armor is drawn on the screen
    public override void Draw(IRenderWindow disp)
        => disp.Draw(Glyph, Pos, ConsoleColor.Cyan);
    // Draw armor at its position with cyan color
}