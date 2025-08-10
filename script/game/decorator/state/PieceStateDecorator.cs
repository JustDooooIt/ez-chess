using System;
using Godot;

public abstract class PieceStateDecorator(IPieceState wrapped) : PieceDecorator<IPieceState>(wrapped), IPieceState
{
}