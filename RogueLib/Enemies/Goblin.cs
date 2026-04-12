using RogueLib.Utilities;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Goblin : Enemy
{
    public override string Name => "Goblin";

    public Goblin(Vector2 pos) : base(pos, 'g', 5) { }

    public override void Update(Player player, TileSet walkables)
    {
        var oldPos = Pos;

        MoveToward(player.Pos, walkables);

        // ✅ ONLY attack if moved INTO player
        if (Pos == player.Pos && oldPos != player.Pos)
        {
            Attack(player, 3);
        }
    }
}