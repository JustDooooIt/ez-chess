using System;
using Godot;

public partial class PieceState : RefCounted, IPieceState
{
  public event Action<int> ActionCompleted;

  public Pipeline StatePipeline { get; set; }
  public Pipeline RenderPipeline { get; set; }

  GodotObject IPiece.Origin => this;

}
