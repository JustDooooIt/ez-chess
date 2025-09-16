using System.Collections.Generic;
using Godot;

public partial class MoveInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator<MoveEvent>(wrapped), IMoveable
{
  protected override void _ReciveEvent(MoveEvent @event)
  {
    var instance = (IPieceInstance)Origin;
    instance.Selectable = false;
    instance.IsRunning = true;
    var tween = ((Node)instance.Origin).CreateTween();
    foreach (var point in @event.path)
    {
      var position = instance.HexMap.ToLocalPosition(point);
      tween.TweenProperty((Node)instance.Origin, "position", position, 0.25);
    }
    tween.TweenCallback(Callable.From(() => { instance.Selectable = true; instance.IsRunning = false; }));
  }
}