using System.Collections.Generic;
using Godot;

public partial class AttackStateDecorator(IPieceState wrapped, List<float> attackPoints) : PieceStateDecorator(wrapped), IAttackable, IAttackEventSender, IFlipable
{
  private int _stateIndex;
  private List<float> AttackPoints { get; set; } = attackPoints;
  private float CurAttackPoint { get; set; } = attackPoints[0];
  public int MaxIndex => AttackPoints.Count;

  public void Attack(Vector2I from, Vector2I to)
  {

  }

  public void Flip()
  {
    _stateIndex++;
    _stateIndex %= AttackPoints.Count;
  }

  public void SendAttackEvent(Vector2I from, Vector2I to)
  {

  }
}