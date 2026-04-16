using RogueLib.Engine;

namespace RogueLib.Dungeon;

public class BaseLevel : Scene {
  protected string? _map;    // dungeon background to be drawn first

  public override void DoCommand(Command command) {
    throw new NotImplementedException();
  }

  public override void Draw(IRenderWindow disp) {
    throw new NotImplementedException();
  }

  public override void Update() {
    throw new NotImplementedException();
  }
}