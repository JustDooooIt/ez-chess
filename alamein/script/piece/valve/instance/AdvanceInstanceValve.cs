public partial class AdvanceInstanceValve(IPieceInstance pieceInstance, AdvanceEvent @event) : InstanceValve(pieceInstance, @event)
{
  private new AdvanceEvent _event = @event;

  protected override void DoLaunch()
  {
    _pieceInstance.Query<IAdvancable>().ReciveEvent(_event);
  }
}