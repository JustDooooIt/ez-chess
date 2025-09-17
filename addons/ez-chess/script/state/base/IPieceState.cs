using Godot;

public interface IPieceState : IPiece
{
  public PieceAdapter PieceAdapter { get; set; }
}