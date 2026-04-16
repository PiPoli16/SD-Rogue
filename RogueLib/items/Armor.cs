using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

/// <summary>
/// Armor item that increases the player's defense (shield value).
/// 
/// Armor is a permanent stat-boosting item that:
/// - Adds defense when picked up
/// - Has different types (Light, Medium, Heavy)
/// - Scales slightly with dungeon level
/// </summary>
public class Armor : Item
{
    /// <summary>
    /// Type of armor determines base defense value.
    /// </summary>
    public ArmorType Type { get; }

    /// <summary>
    /// Final defense value granted when applied to player.
    /// </summary>
    public int DefenseValue { get; }

    /// <summary>
    /// Creates an armor item at a specific position with a type and level scaling.
    /// </summary>
    /// <param name="pos">Position in the dungeon</param>
    /// <param name="type">Armor type (Light, Medium, Heavy)</param>
    /// <param name="level">Dungeon level used for scaling defense</param>
    public Armor(Vector2 pos, ArmorType type, int level)
        : base('[', pos)
    {
        Type = type;

        // Base defense depends on armor type
        DefenseValue = type switch
        {
            ArmorType.Light => 1,
            ArmorType.Medium => 2,
            ArmorType.Heavy => 3,
            _ => 0
        };

        // Level scaling: each level increases defense by +1
        DefenseValue += level - 1;
    }

    /// <summary>
    /// Applies armor effect to player by increasing their defense (shield).
    /// </summary>
    public override void Apply(Player player)
    {
        player.AddArmor(DefenseValue);
    }

    /// <summary>
    /// Draws the armor on the game screen using a cyan color.
    /// </summary>
    public override void Draw(IRenderWindow disp)
        => disp.Draw(Glyph, Pos, ConsoleColor.Cyan);
}