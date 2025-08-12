using Godot;

public class AttackInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator(wrapped), IAttackable
{
  public void Attack(Vector2I from, Vector2I to)
  {

  }
}