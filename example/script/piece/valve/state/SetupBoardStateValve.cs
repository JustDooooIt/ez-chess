using System.Threading.Tasks;

public partial class SetupBoardStateValve(IPieceState pieceState, RenderSetupBoardEvent setupBoardEvent) : StateValve(pieceState)
{
  private RenderSetupBoardEvent _setupBoardEvent = setupBoardEvent;

  protected override Task DoLaunch()
  {
    var position = _setupBoardEvent.position;
    _pieceState.Proxy.As<IPosition>().Position = position;
    _pieceState.PiecesManager.AddPiece(position, _pieceState.PieceAdapter);
    PipelineEventBus.Instance.Publish(GetInstanceId(), _setupBoardEvent);
    return Task.CompletedTask;
  }
}