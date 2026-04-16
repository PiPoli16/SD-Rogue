using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.items;

/// <summary>
/// Defines the different types of potions available in the game.
/// 
/// Each potion type determines what stat or resource is affected
/// when the potion is applied to the player.
/// </summary>
public enum PotionType
{
    /// <summary>
    /// Restores the player's HP.
    /// </summary>
    Healing,

    /// <summary>
    /// Increases the player's strength (attack power).
    /// </summary>
    Power,

    /// <summary>
    /// Increases the player's armor (defense/shield).
    /// </summary>
    Guard
}