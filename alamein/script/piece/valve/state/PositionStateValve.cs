using System.Threading.Tasks;

public partial class PositionStateValve(IPieceState pieceState, PositionEvent @event) : StateValve(pieceState, @event)
{
  protected override void DoLaunch()
  {
    _pieceState.Query<IPositionable>().MapPosition = @event.position;
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
  }
}