using RogueLib.Utilities;
using RogueLib.Enemies;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Troll : Enemy
{
    public Troll(Vector2 pos)
        : base(pos, "Troll", 'T', 20, 15, 5, 1, 6, ticksPerMove: 10) // slower, plus flee
    { }
}