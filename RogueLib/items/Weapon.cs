using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

/// <summary>
/// Weapon item that increases the player's attack strength.
/// 
/// Weapons are permanent stat-boosting items that:
/// - Increase player strength when picked up
/// - Have different types with different base power
/// - Scale with dungeon level
/// </summary>
public class Weapon : Item
{
    /// <summary>
    /// Attack power granted by this weapon.
    /// </summary>
    public int Power { get; }

    /// <summary>
    /// Type of weapon determines base damage and visual style.
    /// </summary>
    public WeaponType Type { get; }

    /// <summary>
    /// Creates a weapon at a given position with type and level scaling.
    /// </summary>
    /// <param name="pos">Position in dungeon</param>
    /// <param name="type">Weapon type (Dagger, Sword, Axe, Bow)</param>
    /// <param name="level">Dungeon level used for scaling damage</param>
    public Weapon(Vector2 pos, WeaponType type, int level)
        : base(')', pos)
    {
        Type = type;

        // Base damage depends on weapon type
        int basePower = type switch
        {
            WeaponType.Dagger => 1,
            WeaponType.Sword => 3,
            WeaponType.Axe => 5,
            WeaponType.Bow => 1,
            _ => 1
        };

        // Level scaling: each dungeon level increases damage by +1
        Power = basePower + (level - 1);
    }

    /// <summary>
    /// Applies weapon effect to player by increasing strength.
    /// </summary>
    public override void Apply(Player player)
    {
        player.AddStrength(Power);
    }

    /// <summary>
    /// Draws the weapon with a color based on its type.
    /// </summary>
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