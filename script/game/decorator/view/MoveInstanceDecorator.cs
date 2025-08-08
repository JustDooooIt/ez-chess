using Godot;

public class MoveInstanceDecorator(PieceInstance wrapped) : PieceInstanceDecorator(wrapped), IMoveable
{
  public void Move(Vector2I from, Vector2I to)
  {
    
  }
}