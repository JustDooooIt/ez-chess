using Godot;
using System;

public partial class HexMap : Sprite2D
{
  private TileMapLayer _baseTerrain;

  public override void _Ready()
  {
	_baseTerrain = GetNode<TileMapLayer>("BaseTerrain");
  }

  public void PlacePiece(Node2D piece, Vector2 vector)
  {
	piece.Position = vector;
  }

  public void PlacePiece(Node2D piece, Vector2I vector)
  {
	piece.Position = _baseTerrain.MapToLocal(vector) + _baseTerrain.Position;
  }
}
