using System.Threading.Tasks;
using Godot;

public partial class RetreatInstanceValve(IPieceInstance pieceInstance, RetreatEvent moveEvent) : InstanceValve(pieceInstance)
{
  private RetreatEvent _moveEvent = moveEvent;

  protected override void DoLaunch()
  {
    _pieceInstance.Proxy.As<IRetreatable>()?.ReciveEvent(_moveEvent);
  }
}
