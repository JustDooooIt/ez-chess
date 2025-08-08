using Godot;

public class MoveStateDecorator(PieceState piece, float movement) : PieceStateDecorator(piece), IMoveable
{
  public float Movement { get; set; } = movement;

  public void Move(Vector2I from, Vector2I to)
  {

  }

  public override T As<T>() where T : class
  {
    if (typeof(T) == typeof(IMoveable))
      return this as T;

    return base.As<T>();
  }
}