public abstract partial class PieceDecorator<TEvent> : PieceDecorator, IAction<TEvent>
    where TEvent : PieceEvent
{
  protected PieceDecorator(IPiece wrapped) : base(wrapped) { }

  protected abstract void DoReciveEvent(TEvent @event);
  protected abstract void SaveOperation(TEvent @event);

  public void ReciveEvent(TEvent @event)
  {
    DoReciveEvent(@event);
    // 判断是否为恢复棋盘阶段,以及判断当前操作者是否为玩家
    if (!@event.recovered && PipelineAdapter is not OtherPipeline)
    {
      SaveOperation(@event);
    }
  }
}