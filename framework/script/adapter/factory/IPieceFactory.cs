using Godot;
using Godot.Collections;

public interface IPieceFactory
{
  public const string PIECE_INSTANCE_PATH = "res://framwork/scene/piece.tscn";
  PieceAdapter Create(int pieceType, int faction, string name, Array<Texture2D> images, int defaultFace, Vector2 areaSize, Array<Dictionary<string, Variant>> property);
}