using System;
using Godot;

public partial class PieceState : RefCounted, IPiece
{
  public event Action<int> ActionCompleted;

  public int Faction { get; set; }
  public Vector2I Position { get; set; }
}