using Godot;
using System;
using System.Collections.Generic;

public partial class PiecesManager : Node
{
  
  public Group<Vector2I, PieceAdapter> Pieces { get; } = new();

  public override void _Ready()
  {

  }

  public void AddPiece(Vector2I position, PieceAdapter piece)
  {
    Pieces.Add(position, piece);
  }
}
