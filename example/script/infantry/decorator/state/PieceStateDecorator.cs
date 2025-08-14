using System;
using Godot;

public abstract class PieceStateDecorator(IPieceState wrapped) : PieceDecorator<IPieceState>(wrapped), IPieceState
{
  public PieceAdapter PieceAdapter { get; set; }
}