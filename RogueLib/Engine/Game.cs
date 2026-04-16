using RogueLib.Dungeon;
using RogueLib.Utilities;

namespace RogueLib.Engine;

// ------------------------------------------------------- 
// To create a new game inherit this class, 
// attach a render window, a player and the first level  
//
// player 
// window
// level 
// ------------------------------------------------------- 

public class Game
{

    protected Scene? _currentLevel;
    protected IRenderWindow? _window;
    protected Player? _player;

    public void run()
    {
        _window ??= new ScreenBuff();

        while (_currentLevel!.IsActive)
        {
            // 1️⃣ Update level (enemies, auto-potion, messages)
            _currentLevel.Update();

            // 2️⃣ Draw map + enemies + items + player + HUD + notifications
            _currentLevel.Draw(_window!);

            // 3️⃣ Display the buffer
            _window!.Display();

            // 4️⃣ Handle player input asynchronously
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (_currentLevel.HasCommand(key))
                    _currentLevel.DoCommand(new Command(_currentLevel.GetCommand(key)));
            }
            else
            {
                // Small delay for CPU
                System.Threading.Thread.Sleep(50);
            }
        }
    }

    // Load a new level
    public void LoadLevel(Scene newLevel)
    {
        _currentLevel = newLevel;
    }
}
