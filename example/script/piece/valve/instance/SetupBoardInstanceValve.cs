using System.Threading.Tasks;
using Godot;

public partial class SetupBoardInstanceValve(IPieceInstance pieceInstance, RenderSetupBoardEvent setupBoardEvent) : InstanceValve(pieceInstance)
{
  private IPieceInstance _piece = pieceInstance;
  private RenderSetupBoardEvent _setupBoardEvent = setupBoardEvent;

  protected override Task DoLaunch()
  {
    var origin = (Node2D)_piece.Origin;
    origin.Position = _piece.TerrainLayers.BaseTerrain.MapToLocal(_setupBoardEvent.position) + _piece.TerrainLayers.Position;
    return Task.CompletedTask;
  }
}