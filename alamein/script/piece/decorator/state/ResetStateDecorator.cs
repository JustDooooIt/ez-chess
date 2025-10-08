[RegisterValve("Reset", ValveTypes.STATE, true)]
public partial class ResetStateDecorator(IPieceState wrapped) : PieceStateDecorator<ResetEvent>(wrapped), IResetable
{

  protected override void _ReciveEvent(ResetEvent @event)
  {
    foreach (var it in ((IInterfaceQueryable)this).Proxy.QueryAll<IResetable>())
    {
      if (it != this)
      {
        it.ReciveEvent(@event);
      }
    }
  }

  protected override Operation _ToOperation(ResetEvent @event) => null;
}