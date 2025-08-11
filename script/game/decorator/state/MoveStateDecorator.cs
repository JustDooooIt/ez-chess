using Godot;

public class MoveStateDecorator(IPieceState piece, float movement) : PieceStateDecorator(piece), IMoveable
{
  public float Movement { get; set; } = movement;

  public void Move(Vector2I from, Vector2I to)
  {
	ulong instance = PieceAdapter.GetInstanceFromState(OriginPiece.GetInstanceId());
	Valve moveValve = new MoveStateValve(OriginPiece, new(instance, from, to));
	GameManager.StatePipeline.AddValve(moveValve);
	GameManager.RenderPipeline.RegisterValve<RenderMoveEvent>(moveValve);
  }

  public override V As<V>() where V : class
  {
	if (typeof(V) == typeof(IMoveable))
	  return this as V;

	return base.As<V>();
  }
}
