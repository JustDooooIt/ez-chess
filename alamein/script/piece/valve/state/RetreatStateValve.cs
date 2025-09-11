public partial class RetreatStateValve(IPieceState pieceState, RetreatEvent @event) : StateValve(pieceState, @event)
{
  protected override void DoLaunch()
  {
    _pieceState.Query<IRetreatable>().ReciveEvent(@event);
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
  }
}