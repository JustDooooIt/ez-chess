
using System.Threading.Tasks;

public partial class MoveStateValve(IPieceState pieceState, RenderMoveEvent moveEvent) : StateValve(pieceState)
{

  protected override void DoLaunch()
  {
    _pieceState.As<IMoveable>().Move(moveEvent.from, moveEvent.to);
    PipelineEventBus.Instance.Publish(GetInstanceId(), moveEvent);
  }
}
