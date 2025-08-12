
using System.Threading.Tasks;

public partial class MoveStateValve(IPieceState pieceState, RenderMoveEvent moveEvent) : StateValve(pieceState)
{
  private RenderMoveEvent _moveEvent = moveEvent;

  protected override void DoLaunch()
  {
    PipelineEventBus.Instance.Publish(GetInstanceId(), _moveEvent);
  }
}
