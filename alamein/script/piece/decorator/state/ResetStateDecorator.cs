public partial class ResetStateDecorator(IPieceState wrapped) : PieceStateDecorator(wrapped), IResetable
{
  public void ReciveEvent(ResetEvent @event)
  {
    foreach (var it in ((IInterfaceQueryable)this).Proxy.QueryAll<IResetable>())
    {
      if (it != this)
      {
        it.ReciveEvent(@event);
      }
    }
  }
}