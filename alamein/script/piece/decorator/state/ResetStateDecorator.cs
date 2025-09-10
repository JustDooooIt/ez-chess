public partial class ResetStateDecorator(IPieceState wrapped) : PieceStateDecorator<ResetEvent>(wrapped), IResetable
{

  protected override void DoReciveEvent(ResetEvent @event)
  {
    foreach (var it in ((IInterfaceQueryable)this).Proxy.QueryAll<IResetable>())
    {
      if (it != this)
      {
        it.ReciveEvent(@event);
      }
    }
  }

  protected override void SaveOperation(ResetEvent @event)
  {
    throw new System.NotImplementedException();
  }
}