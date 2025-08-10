
using System.Threading.Tasks;

public partial class MoveStateValve(IPieceState pieceState, RenderMoveEvent moveEvent) : StateValve(pieceState)
{
  private RenderMoveEvent _moveEvent = moveEvent;

  protected override async Task DoLaunch()
  {
    PipelineEventBus.Instance.Publish(GetInstanceId(), _moveEvent);
  }
}