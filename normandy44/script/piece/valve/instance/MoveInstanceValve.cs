using System.Threading.Tasks;
using Godot;

public partial class MoveInstanceValve(IPieceInstance pieceInstance, MoveEvent moveEvent) : InstanceValve(pieceInstance)
{
  private MoveEvent _moveEvent = moveEvent;

  protected override async void DoLaunch()
  {
    _pieceInstance.Proxy.As<IMoveable>()?.ReciveEvent(_moveEvent);
  }
}
