using System.Threading.Tasks;

public partial class PositionStateValve(IPieceState pieceState, RenderPositionEvent @event) : StateValve(pieceState)
{
  protected override Task DoLaunch()
  {
    _pieceState.As<IPositionable>().MapPosition = @event.position;
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
    return Task.CompletedTask;
  }
}