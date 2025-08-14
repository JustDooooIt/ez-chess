using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node2D
{
  private Node _players;

  public string PlayerId { get; set; } = "Axis";
  public string Operator { get; set; } = "Axis";

  public override void _Ready()
  {
	_players ??= GetNode<Node>("Players");
  }

  public void AddPlayer(PipelineAdapter pipeline)
  {
	_players ??= GetNode<Node>("Players");
	_players.AddChild(pipeline);
  }

  public List<PipelineAdapter> GetPipelines()
  {
	_players ??= GetNode<Node>("Players");
	return [.. _players.GetChildren().Cast<PipelineAdapter>()];
  }

  public void StartPipelines()
  {
	foreach (var adapter in GetPipelines())
	{
	  adapter.StatePipeline.Launch();
	  adapter.RenderPipeline.Launch();
	}
  }
}
