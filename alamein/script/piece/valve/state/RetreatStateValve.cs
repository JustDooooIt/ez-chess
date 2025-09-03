public partial class RetreatStateValve(IPieceState pieceState, RetreatEvent retreatEvent) : StateValve(pieceState)
{
  protected override void DoLaunch()
  {
    _pieceState.As<IRetreatable>().ReciveEvent(retreatEvent);
    PipelineEventBus.Instance.Publish(GetInstanceId(), retreatEvent);
  }
}