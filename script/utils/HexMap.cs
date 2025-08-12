using Godot;
using System;

public partial class HexMap : Sprite2D
{
  private TileMapLayer _baseTerrain;

  public Vector2 MapOffset { get => _baseTerrain.Position; }

  public override void _Ready()
  {
	_baseTerrain = GetNode<TileMapLayer>("BaseTerrain");
  }

  public void PlacePiece(PieceAdapter piece, Vector2 vector)
  {

  }

  public void PlacePiece(PieceAdapter piece, Vector2I iPosition)
  {
	// var position = _baseTerrain.MapToLocal(iPosition) + _baseTerrain.Position;
	piece.State.As<ISetupBoard>().SetupBoard(iPosition);
  }
}
