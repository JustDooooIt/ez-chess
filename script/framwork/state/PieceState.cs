using System;
using Godot;

public partial class PieceState : RefCounted, IPieceState
{
  public event Action<int> ActionCompleted;

  GodotObject IPiece.Origin => this;
  public PipelineAdapter PipelineAdapter { get; set; }
}
