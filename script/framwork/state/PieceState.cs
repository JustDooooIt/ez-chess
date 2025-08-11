using System;
using Godot;

public partial class PieceState : RefCounted, IPieceState
{
  public event Action<int> ActionCompleted;

  public int Faction { get; set; }
  public Vector2I Position { get; set; }
  public Pipeline StatePipeline { get; set; }
  public Pipeline RenderPipeline { get; set; }

  public GodotObject GetWrapped()
  {
    return this;
  }
}