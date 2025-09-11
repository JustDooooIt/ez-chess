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

public record AttackOperation : Operation
{
  public int FromFaction { get; set; }
  public string From { get; set; }
  public int TargetFaction { get; set; }
  public string Target { get; set; }
  public int CombatResult { get; set; }
}

public record RetreatOperation : Operation
{
  public Vector2I From { get; set; }
  public Vector2I To { get; set; }
  public Vector2I[] Path { get; set; }
}

public record DisposeOperation : Operation
{
}

public record AdvanceOperation : Operation
{
  public Vector2I From { get; set; }
  public Vector2I To { get; set; }
}

public enum OperationType
{
  MOVE, ATTACK, RETREAT, DISPOSE, ADVANCE
}
