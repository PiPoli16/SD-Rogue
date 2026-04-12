using RogueLib.Utilities;
using RogueLib.Enemies;
using SandBox01.Levels;

namespace RogueLib.Enemies;

using TileSet = System.Collections.Generic.HashSet<RogueLib.Utilities.Vector2>;

public class Goblin : Enemy
{
    public override string Name => "Goblin";

    public Goblin(Vector2 pos)
        : base(pos, 'g', 5)
    {
    }

    public override void Update(Player player, TileSet walkables)
    {
        MoveToward(player.Pos, walkables);

        if (Pos == player.Pos)
        {
            Attack(player, 3);
        }
    }
}