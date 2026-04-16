using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items;

/// <summary>
/// Special potion that fully restores the player's health.
/// 
/// Unlike normal potions, this item has a unique effect:
/// it instantly heals the player to full HP regardless of current health.
/// </summary>
public class SpecialPotion : Item
{
    /// <summary>
    /// Creates a special potion at a given position.
    /// </summary>
    /// <param name="pos">Position in the dungeon</param>
    public SpecialPotion(Vector2 pos) : base('!', pos) { }

    /// <summary>
    /// Applies full heal effect to the player by restoring HP to MaxHP.
    /// </summary>
    public override void Apply(Player player)
        => player.RestoreMaxHP();

    /// <summary>
    /// Draws the special potion on screen using a dark magenta color.
    /// </summary>
    public override void Draw(IRenderWindow disp)
        => disp.Draw('!', Pos, ConsoleColor.DarkMagenta);
}