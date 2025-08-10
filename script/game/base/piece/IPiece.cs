using System;
using Godot;

public interface IPiece : IActionEvent, IInterfaceQueryable
{
  GodotObject GetOrigin();
  ulong GetInstanceId();
}