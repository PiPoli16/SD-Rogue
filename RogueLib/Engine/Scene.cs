using RogueLib.Engine;
using RogueLib.Utilities;
using System.Collections.Generic;

namespace RogueLib.Dungeon;

public abstract class Scene : ICommandable, IDrawable
{
    public abstract void DoCommand(Command command);
    public abstract void Draw(IRenderWindow disp);
    public abstract void Update();

    protected Player? _player;
    protected Game? _game;

    protected bool _levelActive = true;

    protected Dictionary<ConsoleKey, string> _commandMap = new();

    public bool IsActive => _levelActive;

    public bool HasCommand(ConsoleKey key)
        => _commandMap.ContainsKey(key);

    public string GetCommand(ConsoleKey key)
        => _commandMap[key];

    protected void RegisterCommand(ConsoleKey key, string command)
        => _commandMap[key] = command;
}