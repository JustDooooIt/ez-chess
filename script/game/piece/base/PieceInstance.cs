using System;
using Godot;

public partial class PieceInstance : Node2D, IPieceInstance
{
  public event Action<int> ActionCompleted;

  public Tween Tween { get; private set; }

  public GodotObject GetOrigin()
  {
    return this;
  }

  public override void _Ready()
  {
    Tween = CreateTween();
    Tween.Pause();
  }
}