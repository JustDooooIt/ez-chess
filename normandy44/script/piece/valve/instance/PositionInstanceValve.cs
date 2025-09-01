using System.Threading.Tasks;

public partial class PositionInstanceValve(IPieceInstance pieceInstance, PositionEvent @event) : InstanceValve(pieceInstance)
{
  protected override void DoLaunch()
  {
    _pieceInstance.Proxy.As<IPositionable>().MapPosition = @event.position;
  }
}