using RogueLib.Dungeon;   // Gives access to base Item class
using RogueLib.Utilities; // Gives access to Vector2 (position)

namespace RogueLib.items; // Namespace for all item-related classes

/// <summary>
/// Weapon item that increases the player's attack strength.
/// </summary>
public class Weapon : Item // Inherits from Item (base class)
{
    // Stores how much strength this weapon gives
    public int Power { get; }

    // Stores the type of weapon (Dagger, Sword, Axe, Bow)
    public WeaponType Type { get; }

    // Constructor to create a weapon with position, type, and level scaling
    public Weapon(Vector2 pos, WeaponType type, int level)
        : base(')', pos) // Calls Item constructor → sets Glyph ')' and position
    {
        Type = type; // Save the weapon type

        // Determine base power depending on weapon type
        int basePower = type switch
        {
            WeaponType.Dagger => 1, // weakest weapon
            WeaponType.Sword => 3,  // medium weapon
            WeaponType.Axe => 5,    // strongest weapon
            WeaponType.Bow => 1,    // low base (could be ranged concept)
            _ => 1                 // default fallback
        };

        // Apply level scaling:
        // Example:
        // Level 1 → +0
        // Level 2 → +1
        // Level 3 → +2
        Power = basePower + (level - 1);
    }

    // Defines what happens when player picks up the weapon
    public override void Apply(Player player)
    {
        player.AddStrength(Power); // Increase player's strength
    }

    // Defines how the weapon is drawn on the screen
    public override void Draw(IRenderWindow disp)
    {
        // Choose color based on weapon type
        ConsoleColor color = Type switch
        {
            WeaponType.Dagger => ConsoleColor.Gray,
            WeaponType.Sword => ConsoleColor.White,
            WeaponType.Axe => ConsoleColor.DarkYellow,
            WeaponType.Bow => ConsoleColor.Green,
            _ => ConsoleColor.White
        };

        // Draw the weapon glyph at its position with chosen color
        disp.Draw(Glyph, Pos, color);
    }
}