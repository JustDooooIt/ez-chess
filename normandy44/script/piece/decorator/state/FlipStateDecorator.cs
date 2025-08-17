public partial class FlipStateDecorator(IPieceState wrapped) : PieceStateDecorator(wrapped), IFlipable, IFlipEventSender
{
  public void Flip()
  {
    var decorators = (this as IPiece).QueryAll<IFlipable>()?.GetEnumerator();
    while (decorators.MoveNext())
    {
      decorators.Current.Flip();
    }
  }

  public void SendFlipEvent()
  {
    ulong instance = PieceAdapter.GetInstanceFromState(Wrapped.GetInstanceId());
    Valve valve = new FlipStateValve(this, new(instance));
    PipelineAdapter.StatePipeline.AddValve(valve);
    PipelineAdapter.RenderPipeline.RegisterValve<RenderFlipEvent>(valve);
  }
}