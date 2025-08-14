using Godot;

public class MoveInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator(wrapped), IMoveable
{
  public void Move(Vector2I from, Vector2I to)
  {
    Valve moveValve = new MoveInstanceValve(this, new(0, from, to));
    PipelineAdapter.RenderPipeline.AddValve(moveValve);
  }
}