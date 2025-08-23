using System.Collections.Generic;
using System.Text.Json.Serialization;
using Godot;

public record MoveOperation : Operation
{
  [JsonPropertyName("from")]
  public Vector2I From { get; set; }
  [JsonPropertyName("to")]
  public Vector2I To { get; set; }
  [JsonPropertyName("path")]
  public Vector2I[] Path { get; set; }
}