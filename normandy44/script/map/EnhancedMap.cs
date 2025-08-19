using System.Collections.Generic;
using Godot;

public partial class EnhancedMap : HexMap
{
  [Export]
  public TileMapLayer RiverLayer { get; set; }
  [Export]
  public TileMapLayer RoadLayer { get; set; }

  public override void _Ready()
  {
    base._Ready();
  }

  protected override AStar CreateAStar()
  {
    return new Pathfinder()
    {
      GridWidth = MapBounds.Size.X,
      MapOrigin = MapBounds.Position,
      RiverLayer = RiverLayer,
      RoadLayer = RoadLayer,
    };
  }

  protected override void InitTerrainCost(ref Dictionary<int, float> cost)
  {
    cost[0] = 1;
    cost[1] = 1;
    cost[2] = 1;
    cost[3] = 1;
    cost[4] = 1;
    cost[5] = 1;
    cost[6] = 1;
    cost[7] = 1;
    cost[8] = 1;
    cost[9] = 1;
    cost[10] = 1;
    cost[11] = 1;
    cost[12] = 1;
  }
}