using System.Threading.Tasks;

public partial class FlipStateValve(IPieceState pieceState, FlipEvent @event) : StateValve(pieceState)
{
  protected override void DoLaunch()
  {
    _pieceState.As<IFlipable>().Flip();
    PipelineEventBus.Instance.Publish(GetInstanceId(), @event);
  }
}