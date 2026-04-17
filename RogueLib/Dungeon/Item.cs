using RogueLib.Utilities; // Gives access to Vector2 (position struct)

namespace RogueLib.Dungeon; // Logical grouping for dungeon-related classes

/// <summary>
/// Base class for all items (weapons, potions, armor, gold, etc.)
/// Items exist in the world and can be picked up by the player.
/// This class defines common properties and behavior for all items.
/// </summary>
public abstract class Item // abstract = cannot be instantiated directly
{
    /// <summary>
    /// Position of the item in the dungeon grid.
    /// 
    /// </summary>
    public Vector2 Pos { get; set; } // get = read, set = modify position

    /// <summary>
    /// Character symbol used to represent the item on screen.
    /// Example: '!' = potion, ')' = weapon, '*' = gold
    /// </summary>
    public char Glyph { get; init; } // init = set only during construction

    /// <summary>
    /// Constructor initializes the item’s symbol and position.
    /// </summary>
    protected Item(char glyph, Vector2 pos) // protected = only subclasses can call this
    {
        Glyph = glyph; // assign visual representation
        Pos = pos;     // assign location in the dungeon
    }

    /// <summary>
    /// Defines what happens when the player picks up the item.
    /// This must be implemented by each specific item type.
    /// </summary>
    public abstract void Apply(Player player);
    // abstract = no implementation here, subclasses MUST implement

    /// <summary>
    /// Defines how the item is drawn on the screen.
    /// Each item decides its own color and appearance.
    /// </summary>
    public abstract void Draw(IRenderWindow disp);
    // disp = rendering system (draws to buffer, not directly to console)
}