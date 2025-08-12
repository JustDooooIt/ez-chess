using System;
using Godot;

public interface IPiece : IInterfaceQueryable
{
  GodotObject Origin { get; }
  ulong GetInstanceId();
  public Pipeline StatePipeline { get; set; }
  public Pipeline RenderPipeline { get; set; }
}