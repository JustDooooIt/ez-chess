
using System.Threading.Tasks;

public partial class MoveStateValve(IPieceState pieceState, MoveEvent moveEvent) : StateValve(pieceState)
{

  protected override async void DoLaunch()
  {
    _pieceState.As<IMoveable>().ReciveEvent(moveEvent);
    PipelineEventBus.Instance.Publish(GetInstanceId(), moveEvent);
  }
}
