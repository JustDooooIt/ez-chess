public partial class AttackStateValve(IPieceState pieceState, AttackEvent @event) : StateValve(pieceState)
{
  protected override void DoLaunch()
  {
    _pieceState.As<IAttackable>()?.ReciveEvent(@event);
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
  }
}