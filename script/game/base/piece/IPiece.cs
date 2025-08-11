using System;
using Godot;

public interface IPiece : IInterfaceQueryable
{
  GodotObject GetWrapped();
  ulong GetInstanceId();
}