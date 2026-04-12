using RogueLib.Utilities;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Orc : Enemy
{
    public override string Name => "Orc";

    public Orc(Vector2 pos) : base(pos, 'O', 8) { }

    public override void Update(Player player, TileSet walkables)
    {
        var oldPos = Pos;

        MoveToward(player.Pos, walkables);

        if (Pos == player.Pos && oldPos != player.Pos)
        {
            Attack(player, 5);
        }
    }
}