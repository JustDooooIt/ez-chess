using System.Collections.Generic;
using Godot;

public class MoveStateDecorator(IPieceState piece, List<float> movements) : PieceStateDecorator(piece), IMoveable, IPosition, IReversible<IMoveable>
{
  public List<float> Movements { get; set; } = movements;
  public float CurMovement { get; set; } = movements[0];
  public float ResidualMovement { get; set; } = movements[0];
  public Vector2I Position { get; set; }

  public void Move(Vector2I from, Vector2I to)
  {
    ulong instance = PieceAdapter.GetInstanceFromState(Wrapped.GetInstanceId());
    Valve moveValve = new MoveStateValve(this, new(instance, from, to));
    PipelineAdapter.StatePipeline.AddValve(moveValve);
    PipelineAdapter.RenderPipeline.RegisterValve<RenderMoveEvent>(moveValve);
  }

  public override V As<V>() where V : class
  {
    if (typeof(V) == typeof(IMoveable))
      return this as V;

    return base.As<V>();
  }

  public void Revers(int index)
  {
    CurMovement = Movements[index];
  }
}
