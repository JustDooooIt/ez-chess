
using System.Threading.Tasks;

public partial class MoveStateValve(IPieceState pieceState, MoveEvent moveEvent) : StateValve(pieceState)
{
  private MoveEvent _moveEvent = moveEvent;
  protected override async Task DoLaunch()
  {
    PipelineEventBus.Instance.Publish<MoveEvent>(this.GetInstanceId(), _moveEvent);
  }
}