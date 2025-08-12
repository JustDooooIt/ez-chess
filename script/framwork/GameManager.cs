using Godot;
using System;

public partial class GameManager : Node2D
{
  private Node _players;

  public override void _Ready()
  {
	_players = GetNode<Node>("Players");
  }

  public void AddPlayer(PipelineAdapter pipeline)
  {
	var players = GetNode<Node>("Players");
	players.AddChild(pipeline);
  }
}
