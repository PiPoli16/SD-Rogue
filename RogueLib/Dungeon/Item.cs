using RogueLib.Utilities;

namespace RogueLib.Dungeon;

public abstract class Item
{
    public Vector2 Pos { get; set; }
    public char Glyph { get; init; }

    protected Item(char glyph, Vector2 pos)
    {
        Glyph = glyph;
        Pos = pos;
    }

    public abstract void Apply(Player player);
    public abstract void Draw(IRenderWindow disp);
}