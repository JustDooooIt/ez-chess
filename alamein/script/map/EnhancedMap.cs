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
}
