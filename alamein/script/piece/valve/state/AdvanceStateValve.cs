public partial class AdvanceStateValve(IPieceState pieceState, AdvanceEvent @event) : StateValve(pieceState, @event)
{
  protected override void DoLaunch()
  {
    _pieceState.Query<IAdvancable>().ReciveEvent(@event);
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
  }
}