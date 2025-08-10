using Godot;

public class MoveStateDecorator(IPieceState piece, float movement) : PieceStateDecorator(piece), IMoveable
{
  public float Movement { get; set; } = movement;

  public void Move(Vector2I from, Vector2I to)
  {
    IValve moveValve = new MoveStateValve(Piece, new(from, to));
    GameManager.StatePipeline.AddValve(moveValve);
  }

  public override V As<V>() where V : class
  {
    if (typeof(V) == typeof(IMoveable))
      return this as V;

    return base.As<V>();
  }
}