using System.Threading.Tasks;

public partial class SetupBoardInstanceValve(PieceInstance pieceInstance, RenderSetupBoardEvent setupBoardEvent) : InstanceValve(pieceInstance)
{
  private RenderSetupBoardEvent _setupBoardEvent = setupBoardEvent;

  protected override void DoLaunch()
  {
    _pieceInstance.SetPosition(_setupBoardEvent.position);
  }
}