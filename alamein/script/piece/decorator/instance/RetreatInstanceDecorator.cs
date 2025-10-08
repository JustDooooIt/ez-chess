using System.Collections.Generic;
using Godot;

[RegisterValve("Retreat", ValveTypes.INSTANCE)]
public partial class RetreatInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator<RetreatEvent>(wrapped), IRetreatable
{
  protected override void _ReciveEvent(RetreatEvent @event)
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
    tween.TweenCallback(Callable.From(() =>
    {
      instance.Selectable = true;
      instance.IsRunning = false;
    }));
  }

}