using System;
using System.Collections.Generic;
using RogueLib.Utilities;
using System.Text;

using RogueLib.Dungeon;
namespace SandBox01.Levels;

public class gold: Item
{
    public int Amount { get; init; }

    public gold(Vector2 pos, int amt) : base('*', pos)
    {
        Amount = amt;
    }

    public override void Draw(IRenderWindow disp)
    {
        disp.Draw(Glyph, Pos, ConsoleColor.Yellow);
    }

}
