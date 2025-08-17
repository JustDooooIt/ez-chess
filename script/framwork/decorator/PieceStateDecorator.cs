using System;
using Godot;

public abstract partial class PieceStateDecorator(IPieceState wrapped) : PieceDecorator(wrapped), IPieceState
{
  public PieceAdapter PieceAdapter { get; set; }
}