using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items
{
    public class Armor : Item
    {
        public Armor(Vector2 pos) : base('[', pos) { }
        public override void Apply(Player player) => player.AddArmor();
        public override void Draw(IRenderWindow disp) => disp.Draw('[', Pos, ConsoleColor.Cyan);
    }
}