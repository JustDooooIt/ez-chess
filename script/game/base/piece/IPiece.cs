using System;
using Godot;

public interface IPiece : IInterfaceQueryable
{
  GodotObject GetOrigin();
  ulong GetInstanceId();
}