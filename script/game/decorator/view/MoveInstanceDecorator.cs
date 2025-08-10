using Godot;

public class MoveInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator(wrapped), IMoveable
{

  public void Move(Vector2I from, Vector2I to)
  {
    GD.Print($"Move from {from} to {to}");
  }

}