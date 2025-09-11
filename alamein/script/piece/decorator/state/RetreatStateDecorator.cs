using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class RetreatStateDecorator(IPieceState piece) :
  PieceStateDecorator<RetreatEvent>(piece), IRetreatable, IRetreatRangeProvider, IRetreatEventSender
{
  private HexMap map;
  public int RetreatRange { get; set; } = 0;

  public void SendRetreatEvent(Vector2I from, Vector2I to, bool recovered = false)
  {
    var astar = CreateAStar();
    Vector2I[] path = [.. astar.GetPointPath(Vector2IToId(from), Vector2IToId(to)).Select(e => new Vector2I((int)e.X, (int)e.Y))];
    ulong pieceId = GetPieceId();
    var piece = InstanceFromId(pieceId) as PieceAdapter;
    if (path.Length > 0)
      AddValve<RetreatEvent, RetreatStateValve>(new(PieceAdapter.Faction, PieceAdapter.Name, from, to, path, recovered));
  }

  private long Vector2IToId(Vector2I vec)
  {
    return ((long)vec.X << 32) | (uint)vec.Y;
  }

  public AStar2D CreateAStar()
  {
    AStar2D astar = new();
    map = PieceAdapter.HexMap;
    var baseTerrain = map.GetChild(0).GetNode<TileMapLayer>("BaseTerrain");
    var piecePos = Query<IPositionable>().MapPosition;

    int maxRange = RetreatRange;

    // --- 1. 发现阶段: 找到所有在范围内的瓦片 ---
    // 使用 HashSet 以获得 O(1) 的查找效率，非常适合检查一个瓦片是否在范围内
    var tilesInRange = new HashSet<Vector2I>();
    var queue = new Queue<(Vector2I pos, int dist)>();

    queue.Enqueue((piecePos, 0));
    tilesInRange.Add(piecePos);

    while (queue.Count > 0)
    {
      var (currentPos, currentDist) = queue.Dequeue();

      // 如果已达到最大距离，则不再从这个点向外扩展
      if (currentDist >= maxRange)
      {
        continue;
      }

      // 获取周围的邻居
      var neighbors = baseTerrain.GetSurroundingCells(currentPos);
      foreach (var neighbor in neighbors)
      {
        // 如果这个邻居还没有被访问过
        if (tilesInRange.Add(neighbor)) // .Add() 返回 true 表示添加成功 (之前不存在)
        {
          queue.Enqueue((neighbor, currentDist + 1));
        }
      }
    }

    // 2a. 将所有在范围内且有效的瓦片添加到 A* 图中
    foreach (var tile in tilesInRange)
    {
      // **重要验证**: 确保这个瓦片在地图上是真实存在的，而不是数学计算出的空位置。
      // 同时也可以在这里加入 "IsWalkable" 的判断。
      if (baseTerrain.GetCellSourceId(tile) != -1) // -1 表示该位置没有瓦片
      {
        astar.AddPoint(Vector2IToId(tile), tile);
      }
    }

    // 2b. 连接图中的点
    foreach (var tile in tilesInRange)
    {
      long tileId = Vector2IToId(tile);
      // 确保这个点已经被成功添加到了图中 (通过了上面的验证)
      if (!astar.HasPoint(tileId)) continue;

      var neighbors = baseTerrain.GetSurroundingCells(tile);
      foreach (var neighbor in neighbors)
      {
        long neighborId = Vector2IToId(neighbor);
        // 只有当邻居也在范围内，并且也被添加到了图中时，才连接它们
        if (astar.HasPoint(neighborId))
        {
          astar.ConnectPoints(tileId, neighborId, true);
        }
      }
    }
    return astar;
  }

  protected override void DoReciveEvent(RetreatEvent @event)
  {
    Query<IPositionable>().MapPosition = @event.to;
    PiecesManager.Pieces.Move(@event.from, @event.to, PieceAdapter);
    (PieceAdapter as GeneralPiece).Retreatable = false;
  }

  protected override void SaveOperation(RetreatEvent @event)
  {
    var op = new RetreatOperation()
    {
      From = @event.from,
      To = @event.to,
      Path = @event.path,
      PieceName = PieceAdapter.Name,
      Type = (int)OperationType.RETREAT,
      Faction = PieceAdapter.Faction,
      CommentType = CommentType.GAME_DATA
    };
    GithubUtils.SaveOperation(GameState.Instance.RoomMetaData.Id, op);
  }

}
