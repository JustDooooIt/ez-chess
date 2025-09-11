using System.Threading.Tasks;

public partial class PositionInstanceValve(IPieceInstance pieceInstance, PositionEvent @event) : InstanceValve(pieceInstance, @event)
{
  protected override void DoLaunch()
  {
    _pieceInstance.Proxy.Query<IPositionable>().MapPosition = @event.position;
  }
}