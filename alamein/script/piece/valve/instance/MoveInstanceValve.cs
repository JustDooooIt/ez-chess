using System.Threading.Tasks;
using Godot;

public partial class MoveInstanceValve(IPieceInstance pieceInstance, MoveEvent @event) : InstanceValve(pieceInstance, @event)
{
  private MoveEvent _moveEvent = @event;

  protected override void DoLaunch()
  {
    _pieceInstance.Proxy.Query<IMoveable>()?.ReciveEvent(_moveEvent);
  }
}
