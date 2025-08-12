using System;
using System.Collections.Generic;
using Godot;

public interface IVulnerable : IActionEvent
{
  public static int ActionType = 3;

  void TakeDamage(Vector2I from, Vector2I to);
}