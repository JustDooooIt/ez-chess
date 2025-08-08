using Godot;

public class WalkStateDecorator(PieceState piece) : PieceDecorator(piece), IMoveable
{
  public void Move(Vector2I from, Vector2I to)
  {
    throw new System.NotImplementedException();
  }
}