using System.Collections.Generic;
using Godot;

public partial class MoveStateDecorator(IPieceState piece, List<float> movements) :
  PieceStateDecorator(piece), IMoveable, IMoveEventSender, IFlipable
{
  private int _stateIndex = movements.Count;
  public List<float> Movements { get; set; } = movements;
  public float CurMovement { get; set; } = movements[0];
  public float ResidualMovement { get; set; } = movements[0];

  public void Move(Vector2I from, Vector2I to)
  {
    Wrapped.As<IPositionable>().MapPosition = to;
  }

  public void SendMoveEvent(Vector2I from, Vector2I to)
  {
    ulong instance = GetPieceInstanceId();
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

  public void Flip()
  {
    _stateIndex++;
    _stateIndex %= Movements.Count;
  }

  public void SendFlipEvent() { }
}
