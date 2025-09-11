public partial class AttackStateValve(IPieceState pieceState, AttackEvent @event) : StateValve(pieceState, @event)
{
  protected override void DoLaunch()
  {
    _pieceState.Query<IAttackable>()?.ReciveEvent(@event);
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
  }
}