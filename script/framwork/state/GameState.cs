using System.Collections.Generic;
using Godot;

public partial class GameState : RefCounted
{
  private List<PieceState> _pieces;

  public void AddPiece(PieceState piece)
  { 
    _pieces.Add(piece);
  }
}