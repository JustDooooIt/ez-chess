using System;
using Godot;

public partial class PieceState : RefCounted, IPiece
{
  public event Action<int> ActionCompleted;

  protected int _faction;
  
  public Vector2I Position { get; set; }
  public float AttackPower { get; set; }
  public float DefensePower{ get; set; }
  public float Movement { get; set; }
}