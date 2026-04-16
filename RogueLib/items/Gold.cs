using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

/// <summary>
/// Gold item used as the game's currency pickup.
/// 
/// When collected, it increases the player's gold counter.
/// Unlike other items, it does not affect combat stats directly.
/// </summary>
public class Gold : Item
{
    /// <summary>
    /// Amount of gold granted when picked up.
    /// </summary>
    public int Amount { get; init; }

    /// <summary>
    /// Creates a gold pickup at a specific position.
    /// </summary>
    /// <param name="pos">Position in the dungeon</param>
    /// <param name="amt">Amount of gold contained</param>
    public Gold(Vector2 pos, int amt) : base('*', pos)
    {
        Amount = amt;
    }

    /// <summary>
    /// Applies the gold effect to the player by increasing their gold total.
    /// </summary>
    public override void Apply(Player player)
    {
        player.AddGold(Amount);
    }

    /// <summary>
    /// Draws the gold on screen using a yellow color.
    /// </summary>
    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Yellow);
    }
}