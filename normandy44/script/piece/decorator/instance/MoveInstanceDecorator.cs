using System.Collections.Generic;
using Godot;

public partial class MoveInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator(wrapped), IMoveable
{
  public void Move(Vector2I from, Vector2I to, Vector2I[] path)
  {
    var instance = (IPieceInstance)Origin;
    instance.Selectable = false;
    var tween = ((Node)instance.Origin).CreateTween();
    foreach (var point in path)
    {
      var position = instance.HexMap.ToLocalPosition(point);
      tween.TweenProperty((Node)instance.Origin, "position", position, 0.5);
    }
    tween.TweenCallback(Callable.From(() => instance.Selectable = true));
  }
}