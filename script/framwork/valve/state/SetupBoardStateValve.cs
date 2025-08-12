using System.Threading.Tasks;

public partial class SetupBoardStateValve(IPieceState pieceState, RenderSetupBoardEvent setupBoardEvent) : StateValve(pieceState)
{
  private RenderSetupBoardEvent _setupBoardEvent = setupBoardEvent;

  protected override void DoLaunch()
  {
    var position = _setupBoardEvent.position;
    _pieceState.As<IReadyPiece>().InitialPosition = position;
    PipelineEventBus.Instance.Publish(GetInstanceId(), _setupBoardEvent);
  }
}