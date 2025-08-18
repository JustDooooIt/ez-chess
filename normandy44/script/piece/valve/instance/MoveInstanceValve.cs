using System.Threading.Tasks;
using Godot;

public partial class MoveInstanceValve(IPieceInstance pieceInstance, RenderMoveEvent moveEvent) : InstanceValve(pieceInstance)
{
  private RenderMoveEvent _moveEvent = moveEvent;

  protected override async Task DoLaunch()
  {
    _pieceInstance.Proxy.As<IMoveable>()?.Move(_moveEvent.from, _moveEvent.to);
  }
}
