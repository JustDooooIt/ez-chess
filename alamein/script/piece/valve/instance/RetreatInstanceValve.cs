using System.Threading.Tasks;
using Godot;

public partial class RetreatInstanceValve(IPieceInstance pieceInstance, RetreatEvent @event) : InstanceValve(pieceInstance, @event)
{
  private RetreatEvent _moveEvent = @event;

  protected override void DoLaunch()
  {
    _pieceInstance.Proxy.Query<IRetreatable>()?.ReciveEvent(_moveEvent);
  }
}
