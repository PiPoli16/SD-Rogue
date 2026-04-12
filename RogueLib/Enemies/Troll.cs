using RogueLib.Utilities;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Troll : Enemy
{
    public override string Name => "Troll";

    public Troll(Vector2 pos) : base(pos, 'T', 12) { }

    public override void Update(Player player, TileSet walkables)
    {
        var oldPos = Pos;

        MoveToward(player.Pos, walkables);

        if (Pos == player.Pos && oldPos != player.Pos)
        {
            Attack(player, 7);
        }
    }
}