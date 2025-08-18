using System.Collections.Generic;
using Godot;

public partial class MoveStateDecorator(IPieceState piece, List<float> movements) :
  PieceStateDecorator(piece), IMoveable, IFlipable,IMoveEventSender, IFlipEventSender
{
  private int _stateIndex = movements.Count;
  public List<float> Movements { get; set; } = movements;
  public float CurMovement { get; set; } = movements[0];
  public float ResidualMovement { get; set; } = movements[0];

  public void Move(Vector2I from, Vector2I to)
  {
    As<IPositionable>().MapPosition = to;
  }

  public void SendMoveEvent(Vector2I from, Vector2I to)
  {
    ulong instanceId = GetPieceInstanceId();
    var instance = InstanceFromId(instanceId) as PieceInstance;
    var path = instance.HexMap.FindPath(from, to, ResidualMovement);
    if (path.Length > 0)
    {
      Valve valve = new MoveStateValve(this, new(instanceId, from, to, path));
      PipelineAdapter.StatePipeline.AddValve(valve);
      PipelineAdapter.RenderPipeline.RegisterValve<RenderMoveEvent>(valve);
    }
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
