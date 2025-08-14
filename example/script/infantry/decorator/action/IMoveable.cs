using System;
using Godot;

public interface IMoveable : IActionEvent
{
  public static int ActionType = 0;

  void Move(Vector2I from, Vector2I to);
}