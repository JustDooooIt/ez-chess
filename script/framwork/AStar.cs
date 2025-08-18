using Godot;
using Godot.Collections;

public partial class AStar : AStar2D
{
	public TileMapLayer RiverLayer { get; set; }
	public float RiverCrossingPenalty { get; set; } = 1;
	public int GridWidth { get; set; }
	public Vector2I MapOrigin { get; set; }
	public Dictionary<int, int[]> UnitTypeCostTable { get; set; } = [];
	public int CurUnitType { get; set; } = -1;

	public override float _ComputeCost(long fromId, long toId)
	{
		Vector2I fromCoord = _IdToCoord(fromId);
		Vector2I toCoord = _IdToCoord(toId);

		float moveCost = 1.0f; // 在六边形网格中，所有邻居距离都视为1

		float terrainMultiplier = GetPointWeightScale(toId);
		float finalCost = moveCost * terrainMultiplier;

		if (RiverLayer != null && IsRiverCrossing(fromCoord, toCoord))
		{
			finalCost += RiverCrossingPenalty;
		}

		return finalCost;
	}

	public override float _EstimateCost(long fromId, long toId)
	{
		Vector2I fromCoord = _IdToCoord(fromId);
		Vector2I toCoord = _IdToCoord(toId);
		int dx = Mathf.Abs(toCoord.X - fromCoord.X);
		int dy = Mathf.Abs(toCoord.Y - toCoord.Y);
		return 1.0f * (dx + dy) + (1.414f - 2.0f) * Mathf.Min(dx, dy);
	}

	private Vector2I _IdToCoord(long id)
	{
		int y = (int)(id / GridWidth);
		int x = (int)(id % GridWidth);
		return new Vector2I(x, y) + MapOrigin;
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
