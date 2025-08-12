using System;
using Godot;

public interface IPiece : IInterfaceQueryable
{
  GodotObject GetOrigin();
  ulong GetInstanceId();

  public Pipeline StatePipeline { get; set; }
  public Pipeline RenderPipeline { get; set; }
}