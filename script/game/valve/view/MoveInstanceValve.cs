using System.Threading.Tasks;
using Godot;

public partial class MoveInstanceValve(PieceInstance pieceInstance) : InstanceValve(pieceInstance)
{
  protected override async Task DoLaunch()
  {
    _pieceInstance.Tween.Chain().TweenProperty(_pieceInstance, "position", new Vector2(100, 100), 1);
    await ToSignal(_pieceInstance.Tween, "finished");
  }
}