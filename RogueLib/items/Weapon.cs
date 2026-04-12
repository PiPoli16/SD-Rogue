using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items
{
    public class Weapon : Item
    {
        public int Power { get; }
        public Weapon(Vector2 pos, int power) : base(')', pos) { Power = power; }
        public override void Apply(Player player) => player.AddStrength(Power);
        public override void Draw(IRenderWindow disp) => disp.Draw(')', Pos, ConsoleColor.White);
    }
}
