using System.Threading.Tasks;

public partial class PositionStateValve(IPieceState pieceState, PositionEvent @event) : StateValve(pieceState)
{
  protected override void DoLaunch()
  {
    _pieceState.As<IPositionable>().MapPosition = @event.position;
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
  }
}