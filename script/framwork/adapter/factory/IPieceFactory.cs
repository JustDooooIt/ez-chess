using Godot;
using Godot.Collections;

public interface IPieceFactory
{
  public const string PIECE_ADAPTER_PATH = "res://scene/piece_adapter.tscn";
  public const string PIECE_INSTANCE_PATH = "res://scene/piece.tscn";
  public abstract PieceAdapter Create(int pieceType, params Variant[] args);
}