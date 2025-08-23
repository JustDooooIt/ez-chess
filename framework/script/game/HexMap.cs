using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class HexMap : Sprite2D
{
	private TileMapLayer _baseTerrain;
	private TerrainLayers _layers;
	private TileMapLayer[] _pathfindingLayers;
	private AStar _astar;
	private Dictionary<Vector2I, long> _coordToId = [];
	private Rect2I _mapBounds;

	public TerrainLayers TerrainLayers { get => _layers; }
	public Rect2I MapBounds => _mapBounds;

  [Export]
	protected Godot.Collections.Dictionary<int, float> _terrainCost = [];

	protected virtual AStar CreateAStar()
	{
		return new AStar()
		{
			GridWidth = _mapBounds.Size.X,
			MapOrigin = _mapBounds.Position
		};
	}

	public override void _Ready()
	{
		_layers = GetNode<TerrainLayers>("TerrainLayers");
		_baseTerrain = GetNode<TileMapLayer>("TerrainLayers/BaseTerrain");
		_pathfindingLayers = [.. _layers.GetChildren().Cast<TileMapLayer>()];
		BuildAStarFromTileMap();
	}

	private void BuildAStarFromTileMap()
	{
		GD.Print("Building A* graph...");
		_coordToId.Clear();

		// --- 1. 收集所有层级的唯一单元格坐标和总边界 ---
		var allUsedCells = new HashSet<Vector2I>();

		// 初始化一个无效的边界
		_mapBounds = new Rect2I();

		bool firstLayer = true;
		foreach (TileMapLayer layer in _pathfindingLayers)
		{
			Rect2I layerBounds = layer.GetUsedRect();
			if (!layerBounds.HasArea()) continue;

			if (firstLayer)
			{
				_mapBounds = layerBounds;
				firstLayer = false;
			}
			else
			{
				_mapBounds = _mapBounds.Merge(layerBounds);
			}

			foreach (var cell in layer.GetUsedCells())
			{
				allUsedCells.Add(cell);
			}
		}

		if (!_mapBounds.HasArea())
		{
			GD.PrintErr("No used cells found in any pathfinding layer.");
			return;
		}

		_astar = CreateAStar();

		// --- 2. 为每个唯一坐标计算综合成本并添加点 ---
		foreach (Vector2I cellCoord in allUsedCells)
		{
			float finalCost = 1.0f; // 默认成本为1，如果没有任何带cost的瓦片
			bool isImpassable = false;

			foreach (TileMapLayer layer in _pathfindingLayers)
			{
				TileData tileData = layer.GetCellTileData(cellCoord);
				if (tileData == null || tileData.Terrain == -1) continue;

				float layerCost = _terrainCost[tileData.Terrain];
				// float layerCost = tileData.GetCustomData("cost").AsSingle();

				if (layerCost < 0)
				{
					isImpassable = true;
					break; // 任何一层不可通行，则此格最终不可通行
				}

				// 采用“最高成本优先”策略
				finalCost = Mathf.Max(finalCost, layerCost);
			}

			if (!isImpassable)
			{
				long id = _CoordToId(cellCoord);
				_coordToId[cellCoord] = id;
				Vector2 worldPos = _pathfindingLayers[0].MapToLocal(cellCoord);
				_astar.AddPoint(id, worldPos, finalCost);
			}
		}
		var neighborDirections = new TileSet.CellNeighbor[]
		{
			TileSet.CellNeighbor.RightSide,
			TileSet.CellNeighbor.LeftSide,
			TileSet.CellNeighbor.TopRightSide,
			TileSet.CellNeighbor.TopLeftSide,
			TileSet.CellNeighbor.BottomRightSide,
			TileSet.CellNeighbor.BottomLeftSide
		};
		// --- 3. 连接所有相邻的可通行节点 ---
		foreach (var (coord, id) in _coordToId)
		{
			foreach (var direction in neighborDirections)
			{
				// 使用内置函数获取邻居坐标
				Vector2I neighborCoord = _baseTerrain.GetNeighborCell(coord, direction);

				// 检查这个邻居坐标是否在我们的可通行点字典中
				if (_coordToId.TryGetValue(neighborCoord, out long neighborId))
				{
					_astar.ConnectPoints(id, neighborId);
				}
			}
		}
		GD.Print($"A* graph built with {_astar.GetPointCount()} points.");
	}

	// 将 TileMap 坐标转换为唯一的 long 类型 ID
	private long _CoordToId(Vector2I coord)
	{
		int relativeX = coord.X - _mapBounds.Position.X;
		int relativeY = coord.Y - _mapBounds.Position.Y;
		return (long)relativeY * _mapBounds.Size.X + relativeX;
	}

	private Vector2I _IdToCoord(long id)
	{
		int y = (int)(id / _mapBounds.Size.X);
		int x = (int)(id % _mapBounds.Size.X);
		return new Vector2I(x, y) + _mapBounds.Position;
	}

	public Vector2I[] FindPath(Vector2I startCoord, Vector2I endCoord, float movementPoints)
	{
		// 1. 检查起点和终点是否是有效的、可通行的地块
		if (!_coordToId.ContainsKey(startCoord) || !_coordToId.ContainsKey(endCoord))
		{
			GD.PrintErr("Start or end point is not on a valid, passable tile.");
			return [];
		}

		long startId = _coordToId[startCoord];
		long endId = _coordToId[endCoord];

		// 2. 使用A*算法获取最短路径的ID数组
		long[] idPath = _astar.GetIdPath(startId, endId);

		// 3. 如果找不到路径，直接返回空数组
		if (idPath.Length == 0)
		{
			GD.Print("No path found between the points.");
			return [];
		}

		// 4. 计算路径的总成本
		float totalCost = 0.0f;
		for (int i = 0; i < idPath.Length - 1; i++)
		{
			totalCost += _astar._ComputeCost(idPath[i], idPath[i + 1]);
		}

		// 5. 比较总成本和移动力，并返回相应结果
		if (totalCost <= movementPoints)
		{
			GD.Print($"Path found with cost {totalCost} (within movement limit {movementPoints}).");
			// 将ID路径转换为坐标路径并返回
			return [.. idPath.Select(_IdToCoord)];
		}
		else
		{
			GD.Print($"Path found, but its cost {totalCost} exceeds movement limit {movementPoints}.");
			// 成本超出移动力，返回空数组
			return [];
		}
	}

	public bool CanReach(Vector2I startCoord, Vector2I endCoord, float movementPoints)
	{
		// 调用辅助函数获取路径成本
		float cost = GetPathCost(startCoord, endCoord);

		// 如果成本小于等于移动力，则可以到达
		// 注意：如果GetPathCost返回PositiveInfinity，这个比较会自动失败
		return cost <= movementPoints;
	}

	private float GetPathCost(Vector2I startCoord, Vector2I endCoord)
	{
		// 1. 检查起点和终点是否是有效的、可通行的地块
		if (!_coordToId.ContainsKey(startCoord) || !_coordToId.ContainsKey(endCoord))
		{
			// 无效地块，无法到达
			return float.PositiveInfinity;
		}

		long startId = _coordToId[startCoord];
		long endId = _coordToId[endCoord];

		// 2. 使用A*算法获取最短路径的ID数组
		long[] idPath = _astar.GetIdPath(startId, endId);

		// 3. 如果找不到路径，则无法到达
		if (idPath.Length == 0)
		{
			return float.PositiveInfinity;
		}

		// 4. 计算并返回路径的总成本
		float totalCost = 0.0f;
		for (int i = 0; i < idPath.Length - 1; i++)
		{
			totalCost += _astar._ComputeCost(idPath[i], idPath[i + 1]);
		}

		return totalCost;
	}

	public Vector2I[] FindPath(Vector2I startCoord, Vector2I endCoord)
	{
		// 1. 必须检查坐标是否在我们的A*图中，否则会引发异常。
		// 这是一个有效性检查，而不是游戏逻辑检查。
		if (!_coordToId.ContainsKey(startCoord) || !_coordToId.ContainsKey(endCoord))
		{
			GD.PrintErr("FindPath: Start or end point is not on a valid, passable tile.");
			return [];
		}

		// 2. 将坐标转换为A*的ID
		long startId = _coordToId[startCoord];
		long endId = _coordToId[endCoord];

		// 3. 调用A*获取ID路径。如果找不到路径，A*会返回一个空数组。
		long[] idPath = _astar.GetIdPath(startId, endId);

		// 4. 将ID路径转换回坐标路径并返回。
		// 如果idPath为空，此操作将正确地返回一个空的Vector2I数组。
		return [.. idPath.Select(_IdToCoord)];
	}

	public Vector2I ToMapPosition(Vector2 pos) { return _baseTerrain.LocalToMap(pos - _layers.Position); }

	public Vector2 ToLocalPosition(Vector2I pos) { return _baseTerrain.MapToLocal(pos) + _layers.Position; }

	public void PlacePiece(PieceAdapter piece, Vector2I mapPosition)
	{
		var localPosition = ToLocalPosition(mapPosition);
		((Node2D)piece.Instance.Origin).Position = localPosition;
	}
}
