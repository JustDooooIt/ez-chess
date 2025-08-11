using System;
using Godot;

public partial class PieceInstance : Node2D, IPieceInstance
{
  public event Action<int> ActionCompleted;

  private Tween _tween;

  public Tween Tween { get => GetTween(); }

  public GodotObject GetWrapped()
  {
    return this;
  }

  private Tween GetTween()
  {
    if (_tween == null)
    {
      return CreateTween();
    }
    else
    {
      if (_tween.IsValid())
        return _tween;
      else
        return CreateTween();
    }
  }
}