using Godot;
using System;
using System.Collections.Generic;

public partial class PiecesManager : Node
{
  private Group<Vector2I, PieceAdapter> _pieces = new();

  public override void _Ready()
  {
	ChildEnteredTree += OnPieceAdded;
  }

  public void OnPieceAdded(Node node)
  {
	var piece = (PieceAdapter)node;
	GD.Print();
  }
}
