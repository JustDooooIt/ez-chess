using Godot;

public partial class AdvanceInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator<AdvanceEvent>(wrapped), IAdvancable
{
  protected override void _ReciveEvent(AdvanceEvent @event)
  {
    var instance = (IPieceInstance)Origin;
    instance.Selectable = false;
    instance.IsRunning = true;
    var tween = ((Node)instance.Origin).CreateTween();
    var position = instance.HexMap.ToLocalPosition(@event.to);
    tween.TweenProperty((Node)instance.Origin, "position", position, 0.25);
    tween.TweenCallback(Callable.From(() => { instance.Selectable = true; instance.IsRunning = false; }));
  }
}