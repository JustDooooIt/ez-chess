using System.Threading.Tasks;
using Godot;

public partial class MoveInstanceValve(IPieceInstance pieceInstance, RenderMoveEvent moveEvent) : InstanceValve(pieceInstance)
{
  private RenderMoveEvent _moveEvent = moveEvent;

  protected override async Task DoLaunch()
  {
    var instance = (PieceInstance)_pieceInstance;
    var position = instance.TerrainLayers.ToLocalPosition(_moveEvent.to);
    _pieceInstance.Tween.TweenProperty(instance, "position", position, 1);
    await ToSignal(instance.Tween, "finished");
    _pieceInstance.Tween.Kill();
    _pieceInstance.Tween = null;
  }
}
