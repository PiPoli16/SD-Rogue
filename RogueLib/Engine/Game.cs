using RogueLib.Dungeon;

namespace RogueLib.Engine;

public class Game
{
    protected Scene? _currentLevel;
    protected IRenderWindow? _window;

    // ✅ ADD THIS LINE (THIS IS YOUR BUG FIX)
    protected Player? _player;

    public void run()
    {
        while (_currentLevel!.IsActive)
        {
            _currentLevel.Draw(_window!);
            _window!.Display();

            HandleInput();

            _currentLevel.Update();
        }
    }

    protected virtual void HandleInput()
    {
        var key = Console.ReadKey(true);

        if (_currentLevel!.HasCommand(key.Key))
        {
            _currentLevel.DoCommand(
                new Command(_currentLevel.GetCommand(key.Key))
            );
        }
    }

    // ✅ LEVEL SWITCHING
    public void LoadLevel(Scene newLevel)
    {
        _currentLevel = newLevel;
    }
}