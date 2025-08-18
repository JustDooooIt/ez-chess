using Godot;

public partial class AttackInstanceDecorator : PieceInstanceDecorator, IAttackable, IAttackEventSender
{
  private IPieceInstance _wrapped;
  
  public AttackInstanceDecorator(IPieceInstance wrapped) : base(wrapped)
  {
    _wrapped = wrapped;
  }

  public void Attack(Vector2I from, Vector2I to)
  {
    throw new System.NotImplementedException();
  }

  public void SendAttackEvent(Vector2I from, Vector2I to)
  {
    throw new System.NotImplementedException();
  }
}