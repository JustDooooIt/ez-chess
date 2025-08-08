using Godot;

public class AttackStateDecorator(PieceState piece, float attackPower) : PieceStateDecorator(piece), IAttackable
{
  public float AttackPower { get; set; } = attackPower;

  public void Attack(Vector2I from, Vector2I to)
  {

  }

  public override T As<T>() where T : class
  {
    if (typeof(T) == typeof(IAttackable))
      return this as T;

    return base.As<T>();
  }
}