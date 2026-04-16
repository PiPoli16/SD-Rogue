using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.items;

/// <summary>
/// Defines the different types of armor available in the game.
/// 
/// Each armor type determines the base amount of defense
/// granted to the player when picked up.
/// </summary>
public enum ArmorType
{
    /// <summary>
    /// Light armor: lowest defense, but usually intended for early game.
    /// </summary>
    Light,

    /// <summary>
    /// Medium armor: balanced defense value.
    /// </summary>
    Medium,

    /// <summary>
    /// Heavy armor: highest defense, used for stronger protection.
    /// </summary>
    Heavy
}