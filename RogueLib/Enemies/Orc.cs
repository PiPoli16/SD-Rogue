using RogueLib.Utilities;
using RogueLib.Enemies;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Orc : Enemy
{
    public Orc(Vector2 pos)
        : base(pos, "Orc", 'o', 10, 8, 3, 1, 4, ticksPerMove: 7)
    { }
}