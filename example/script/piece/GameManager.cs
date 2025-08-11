using Godot;
using System;

public partial class GameManager : Node2D
{
  public static Pipeline StatePipeline { get; private set; }
  public static Pipeline RenderPipeline { get; private set; }

  public override void _Ready()
  {
	StatePipeline = GetNode<Pipeline>("Pipelines/state");
	RenderPipeline = GetNode<Pipeline>("Pipelines/render");
	StatePipeline.Launch();
	RenderPipeline.Launch();

	// Test
	Move();
  }

  private void Move()
  {
	GetNode<GeneralPiece>("Node/GeneralPiece").Move();
  }
}
