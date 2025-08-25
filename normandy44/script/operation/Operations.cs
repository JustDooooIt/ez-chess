using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Godot;

public record MoveOperation : Operation
{
  public Vector2I From { get; set; }
  public Vector2I To { get; set; }
  public Vector2I[] Path { get; set; }
}

public enum OperationType
{
  MOVE
}
