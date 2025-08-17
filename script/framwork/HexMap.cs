using Godot;
using System;

public partial class HexMap : Sprite2D
{
  private TileMapLayer _baseTerrain;
  private TerrainLayers _layers;

  public Vector2 MapOffset { get => _baseTerrain.Position; }

  public override void _Ready()
  {
	_layers = GetNode<TerrainLayers>("TerrainLayers");
	_baseTerrain = GetNode<TileMapLayer>("TerrainLayers/BaseTerrain");
  }

  public Vector2I ToMapPosition(Vector2 pos) { return _baseTerrain.LocalToMap(pos + _layers.Position); }

  public Vector2 ToLocalPosition(Vector2I pos) { return _baseTerrain.MapToLocal(pos) + _layers.Position; }

  public void PlacePiece(PieceAdapter piece, Vector2I mapPosition)
  {
	var localPosition = ToLocalPosition(mapPosition);
	((Node2D)piece.Instance.Origin).Position = localPosition;
  }
}
