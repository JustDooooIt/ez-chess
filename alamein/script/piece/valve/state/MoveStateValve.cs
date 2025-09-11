
using System.Threading.Tasks;

public partial class MoveStateValve(IPieceState pieceState, MoveEvent @event) : StateValve(pieceState, @event)
{

  protected override void DoLaunch()
  {
    _pieceState.Query<IMoveable>().ReciveEvent(@event);
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
  }
}
