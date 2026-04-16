// Import basic system libraries
using System;
using System.Collections.Generic;
using System.Text;

// Namespace used to organize message-related classes
namespace RogueLib.Message;

// This class represents a message shown to the player (like damage, pickup, etc.)
public class Message
{
    // ================= PROPERTIES =================

    // The text content of the message (what will be displayed)
    // "get" only → means it is read-only after being set in the constructor
    public string Text { get; }

    // The color used when displaying the message in the console
    public ConsoleColor Color { get; }

    // ================= PRIVATE FIELD =================

    // TTL = "Time To Live"
    // This determines how long the message stays visible (in frames/ticks)
    private int _ttl;

    // ================= STATE CHECK =================

    // Property that tells whether the message is still active (should be displayed)
    // If _ttl is greater than 0 → message is still alive
    // If _ttl reaches 0 → message should be removed
    public bool IsAlive => _ttl > 0;

    // ================= CONSTRUCTOR =================

    // Creates a new message
    // text → what the message says
    // color → how it appears visually
    // ttl → how long it lasts (default = 40 frames if not specified)
    public Message(string text, ConsoleColor color, int ttl = 40)
    {
        Text = text;     // Set message text
        Color = color;   // Set display color
        _ttl = ttl;      // Set lifetime duration
    }

    // ================= UPDATE =================

    // This method is called every game frame (tick)
    // It decreases the TTL so the message eventually disappears
    public void Update()
    {
        // Only decrease TTL if it's still above 0
        if (_ttl > 0)
            _ttl--; // Reduce remaining lifetime by 1 frame
    }
}