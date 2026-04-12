using RogueLib.Utilities;
using RogueLib.Enemies;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Orc : Enemy
{
    public override string Name => "Orc";

    public Orc(Vector2 pos)
        : base(pos, 'O', 8)
    {
    }

    public override void Update(Player player, TileSet walkables)
    {
        MoveToward(player.Pos, walkables);

        if (Pos == player.Pos)
        {
            Attack(player, 5);
        }
    }
}