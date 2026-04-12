using RogueLib.Utilities;
using RogueLib.Enemies;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Goblin : Enemy
{
    public Goblin(Vector2 pos)
        : base(pos, "Goblin", 'g', 5, 5, 1, 1, 2, ticksPerMove: 5) // faster
    { }
}