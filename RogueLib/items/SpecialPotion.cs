using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.items
{
    public class SpecialPotion : Item
    {
        public SpecialPotion(Vector2 pos) : base('!', pos) { }
        public override void Apply(Player player)
        {
            player.RestoreMaxHP();

            player.AddPotionUsed();
            player.AddLog("Full heal potion used");
        }
        public override void Draw(IRenderWindow disp) => disp.Draw('!', Pos, ConsoleColor.DarkMagenta);
    }
}