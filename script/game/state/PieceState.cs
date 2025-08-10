using System;
using Godot;

public partial class PieceState : RefCounted, IPieceState
{
  public event Action<int> ActionCompleted;

  public int Faction { get; set; }
  public Vector2I Position { get; set; }

  public GodotObject GetOrigin()
  {
    return this;
  }
}