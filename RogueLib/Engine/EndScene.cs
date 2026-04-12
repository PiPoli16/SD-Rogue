using RogueLib.Engine;
using RogueLib.Utilities;

namespace RogueLib.Dungeon;

public class EndScene : Scene
{
    public EndScene(Player player)
    {
        _player = player;
    }

    public override void DoCommand(Command command) { }

    public override void Update() { }

    public override void Draw(IRenderWindow disp)
    {
        var p = _player!;

        int score =
            p.Kills * 100 +
            p.PotionsUsed * 10 +
            p.HP * 5 +
            p.Gold * 2 -
            p.DamageTakenTotal;

        disp.Draw("=== YOU DIED ===", new Vector2(25, 8), ConsoleColor.Red);

        disp.Draw($"Kills: {p.Kills}", new Vector2(25, 10), ConsoleColor.White);
        disp.Draw($"Potions Used: {p.PotionsUsed}", new Vector2(25, 11), ConsoleColor.White);
        disp.Draw($"HP Left: {p.HP}", new Vector2(25, 12), ConsoleColor.White);
        disp.Draw($"Gold: {p.Gold}", new Vector2(25, 13), ConsoleColor.Yellow);
        disp.Draw($"Damage Taken: {p.DamageTakenTotal}", new Vector2(25, 14), ConsoleColor.Red);

        disp.Draw($"FINAL SCORE: {score}", new Vector2(25, 16), ConsoleColor.Magenta);
    }
}