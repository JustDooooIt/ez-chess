using System.Threading.Tasks;

public partial class FlipStateValve(IPieceState pieceState, RenderFlipEvent @event) : StateValve(pieceState)
{
  protected override Task DoLaunch()
  {
    _pieceState.As<IFlipable>().Flip();
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
    return Task.CompletedTask;
  }
}