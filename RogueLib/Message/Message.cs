using System;
using System.Collections.Generic;
using System.Text;

namespace RogueLib.Message;

public class Message
{
    public string Text { get; }
    public ConsoleColor Color { get; }
    private int _ttl; // time to live (frames)

    public bool IsAlive => _ttl > 0;

    public Message(string text, ConsoleColor color, int ttl = 40)
    {
        Text = text;
        Color = color;
        _ttl = ttl;
    }

    public void Update()
    {
        if (_ttl > 0)
            _ttl--;
    }
}
