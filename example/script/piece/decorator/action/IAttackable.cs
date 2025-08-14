using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public interface IAttackable : IActionEvent
{
  public static int ActionType = 1;
  
  void Attack(Vector2I from, Vector2I to);
}