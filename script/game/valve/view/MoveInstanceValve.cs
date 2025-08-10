using System.Threading.Tasks;
using Godot;

public partial class MoveInstanceValve(PieceInstance pieceInstance, RenderMoveEvent moveEvent) : InstanceValve(pieceInstance)
{
  private RenderMoveEvent _moveEvent = moveEvent;

  protected override async Task DoLaunch()
  {
    _pieceInstance.Tween.Chain().TweenProperty(_pieceInstance, "position", new Vector2(_moveEvent.to.X, _moveEvent.to.Y), 1);
  }
}