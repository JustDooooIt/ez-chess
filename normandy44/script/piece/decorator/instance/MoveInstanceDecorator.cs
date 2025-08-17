using Godot;

public partial class MoveInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator(wrapped), IMoveable
{
  private IPieceInstance _wrapped = wrapped;
  public void Move(Vector2I from, Vector2I to)
  {
    var instance = _wrapped;
    var position = instance.HexMap.ToLocalPosition(to);
    instance.Selectable = false;
    var tween = ((Node)instance).CreateTween();
    tween.TweenProperty((Node)instance, "position", position, 1);
    tween.TweenCallback(Callable.From(() => instance.Selectable = true));
  }
  
  public void SendMoveEvent(Vector2I from, Vector2I to)
  {
    Valve moveValve = new MoveInstanceValve(this, new(Wrapped.GetInstanceId(), from, to));
    PipelineAdapter.RenderPipeline.AddValve(moveValve);
  }
}