using Godot;

public partial class Pathfinder : AStar
{

	public float RiverCrossingPenalty { get; set; } = 1;
	public TileMapLayer RiverLayer { get; set; }
	public TileMapLayer RoadLayer { get; set; }

	public override float _ComputeCost(long fromId, long toId)
	{
		float finalCost = base._ComputeCost(fromId, toId);
		Vector2I fromCoord = _IdToCoord(fromId);
		Vector2I toCoord = _IdToCoord(toId);
		var roadTile = RoadLayer.GetCellTileData(toCoord);
		if (roadTile == null)
		{
			if (RiverLayer != null && IsRiverCrossing(fromCoord, toCoord))
			{
				finalCost += RiverCrossingPenalty;
			}
		}
		else
		{
			if (GameState.Instance.CurOperatorFaction == 0)
			{
				var pieceType = GameState.Instance.SelectedPiece.PieceType;
				if (pieceType == 0)
				{
					finalCost = 0.5f;
				}
			}
		}

		return finalCost;
	}
	private bool IsRiverCrossing(Vector2I from, Vector2I to)
	{
		// 定义六边形的所有六个邻居方向
		var neighborDirections = new TileSet.CellNeighbor[]
		{
			TileSet.CellNeighbor.RightSide,
			TileSet.CellNeighbor.LeftSide,
			TileSet.CellNeighbor.TopRightSide,
			TileSet.CellNeighbor.TopLeftSide,
			TileSet.CellNeighbor.BottomRightSide,
			TileSet.CellNeighbor.BottomLeftSide
		};

		// 遍历所有六个方向
		foreach (var directionBit in neighborDirections)
		{
			var tile = RiverLayer.GetCellTileData(from);

			// 获取 'from' 单元格在该方向上的邻居坐标
			Vector2I neighborCoord = RiverLayer.GetNeighborCell(from, directionBit);

			// 如果这个邻居的坐标就是我们的目标 'to' 坐标
			if (neighborCoord == to)
			{
				// 那么我们就找到了移动的方向。现在检查这个方向上是否有 peer bit。
				// 如果有 peer bit，说明是跨河，返回 true。
				if (tile != null && tile.IsValidTerrainPeeringBit(directionBit) && tile.GetTerrainPeeringBit(directionBit) != -1)
				{
					return true;
				}

				// 既然已经找到了正确的方向，就没必要再检查其他方向了。
				// 无论是否有 peer bit，我们都可以直接跳出循环。
				break;
			}
		}

		// 如果遍历完所有方向都没有找到匹配的，或者找到了但没有 peer bit，
		// 那么就不是跨河。
		return false;
	}
}
