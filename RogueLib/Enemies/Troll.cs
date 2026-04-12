using RogueLib.Utilities;
using RogueLib.Enemies;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Troll : Enemy
{
    public override string Name => "Troll";

    public Troll(Vector2 pos)
        : base(pos, 'T', 12)
    {
    }

    public override void Update(Player player, TileSet walkables)
    {
        MoveToward(player.Pos, walkables);

        if (Pos == player.Pos)
        {
            Attack(player, 7);
        }
    }
}