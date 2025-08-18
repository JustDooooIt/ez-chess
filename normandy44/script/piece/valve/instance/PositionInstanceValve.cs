using System.Threading.Tasks;

public partial class PositionInstanceValve(IPieceInstance pieceInstance, RenderPositionEvent @event) : InstanceValve(pieceInstance)
{
  protected override Task DoLaunch()
  {
    _pieceInstance.Proxy.As<IPositionable>().MapPosition = @event.position;
    return Task.CompletedTask;
  }
}