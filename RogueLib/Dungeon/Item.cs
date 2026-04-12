using System;
using System.Collections.Generic;
using RogueLib.Utilities;
using System.Text;

namespace RogueLib.Dungeon;

public abstract class Item
{
    public Vector2 Pos { get; set; }
    public char Glyph { get; init; }

    public Item(char glyph, Vector2 pos)
    {
        Glyph = glyph;
        Pos = pos;
    }

    public abstract void Apply(Player player);
    public abstract void Draw(IRenderWindow disp);
}
