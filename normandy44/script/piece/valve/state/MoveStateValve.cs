
using System.Threading.Tasks;

public partial class MoveStateValve(IPieceState pieceState, RenderMoveEvent moveEvent) : StateValve(pieceState)
{
  private RenderMoveEvent _moveEvent = moveEvent;

  protected override Task DoLaunch()
  {
    _pieceState.As<IMoveable>().Move(_moveEvent.from, _moveEvent.to);
    PipelineEventBus.Instance.Publish(GetInstanceId(), _moveEvent);
    return Task.CompletedTask;
  }
}
