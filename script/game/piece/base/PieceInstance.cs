using System;
using Godot;

public partial class PieceInstance : Node2D, IPieceInstance
{
  public Tween Tween { get; private set; }

  public event Action<int> ActionCompleted;

  public override void _Ready()
  {
    Tween = CreateTween();
    Tween.Pause();
  }
}