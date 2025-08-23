using System.Collections.Generic;
using Godot;

public interface IMoveable
{
  void Move(Vector2I from, Vector2I to, Vector2I[] path);
}