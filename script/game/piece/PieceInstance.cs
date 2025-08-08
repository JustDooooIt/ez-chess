using System;
using Godot;

public partial class PieceInstance : Node2D, IPiece
{
  public event Action<int> ActionCompleted;
  public IPiece.PieceTypes PieceType => IPiece.PieceTypes.VIEW;
}