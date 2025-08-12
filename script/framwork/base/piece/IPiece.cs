using System;
using Godot;

public interface IPiece : IInterfaceQueryable
{
  GodotObject Origin { get; }
  ulong GetInstanceId();
  public PipelineAdapter PipelineAdapter { get; set; }
}