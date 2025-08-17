using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TerrainLayers : Node2D
{
	public TileMapLayer BaseTerrain => GetChild<TileMapLayer>(0);

	public int[] GetTerrainType(Vector2I position)
	{
		List<int> types = [];
		foreach (var mapLayer in GetChildren().Cast<TileMapLayer>())
		{
			var tileData = mapLayer.GetCellTileData(position);
			if (tileData != null)
				types.Add(tileData.Terrain);
		}
		return [.. types];
	}

}
