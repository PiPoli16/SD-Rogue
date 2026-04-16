using RogueLib.Utilities;

namespace RogueLib.Dungeon;

/// <summary>
/// Base class for all items (weapons, potions, armor, gold, etc.)
/// Items exist in the world at a position and can be picked up by the player.
/// Each item defines:
/// - What happens when the player picks it up (Apply)
/// - How it is displayed on screen (Draw)
/// </summary>
public abstract class Item
{
    /// <summary>
    /// Position of the item in the dungeon grid.
    /// Used for collision detection with the player.
    /// </summary>
    public Vector2 Pos { get; set; }

    /// <summary>
    /// Character used to visually represent the item on the map.
    /// Example: '!' for potions, ')' for weapons, '*' for gold.
    /// </summary>
    public char Glyph { get; init; }

    /// <summary>
    /// Constructor sets the item type (glyph) and its position in the world.
    /// </summary>
    /// <param name="glyph">Visual representation of the item</param>
    /// <param name="pos">Position in the dungeon grid</param>
    protected Item(char glyph, Vector2 pos)
    {
        Glyph = glyph;
        Pos = pos;
    }

    /// <summary>
    /// Defines the effect of the item when the player picks it up.
    /// Example:
    /// - Potion restores HP
    /// - Weapon increases strength
    /// - Armor increases defense
    /// - Gold increases currency
    /// </summary>
    public abstract void Apply(Player player);

    /// <summary>
    /// Defines how the item is drawn on the screen.
    /// Each subclass decides its color and appearance.
    /// </summary>
    public abstract void Draw(IRenderWindow disp);
}