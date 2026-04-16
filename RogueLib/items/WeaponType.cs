using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.items;

/// <summary>
/// Defines the different types of weapons available in the game.
/// 
/// Each weapon type determines the base attack power and visual style
/// used when the weapon is drawn in the dungeon.
/// </summary>
public enum WeaponType
{
    /// <summary>
    /// Dagger: low base damage, represents a fast/weak weapon type.
    /// </summary>
    Dagger,

    /// <summary>
    /// Sword: balanced weapon with moderate damage.
    /// </summary>
    Sword,

    /// <summary>
    /// Axe: high damage weapon with stronger base attack.
    /// </summary>
    Axe,

    /// <summary>
    /// Bow: low base damage weapon (can represent ranged-type design in future extensions).
    /// </summary>
    Bow
}