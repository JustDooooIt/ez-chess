using System.Collections.Generic;
using Godot;

public class AttackStateDecorator(IPieceState piece, List<float> attackPowers) : PieceStateDecorator(piece), IAttackable, IReversible<IAttackable>
{
  public List<float> AttackPowers { get; set; } = attackPowers;
  public float CurAttackPower { get; set; } = attackPowers[0];
  
  public void Attack(Vector2I from, Vector2I to)
  {
    GD.Print($"Attack from {from} to {to}");
  }

  public override T As<T>() where T : class
  {
    if (typeof(T) == typeof(IAttackable))
      return this as T;

    return base.As<T>();
  }

  public void Revers(int index)
  {
    CurAttackPower = AttackPowers[index];
  }
}