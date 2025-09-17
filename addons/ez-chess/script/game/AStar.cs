using Godot;
using Godot.Collections;

public partial class AStar : AStar2D
{
	public int GridWidth { get; set; }
	public Vector2I MapOrigin { get; set; }

	public override float _ComputeCost(long fromId, long toId)
	{
		float moveCost = 1.0f; // 在六边形网格中，所有邻居距离都视为1

		float terrainMultiplier = GetPointWeightScale(toId);
		float finalCost = moveCost * terrainMultiplier;

		return finalCost;
	}

	public override float _EstimateCost(long fromId, long toId)
	{
		Vector2I fromCoord = _IdToCoord(fromId);
		Vector2I toCoord = _IdToCoord(toId);
		int dx = Mathf.Abs(toCoord.X - fromCoord.X);
		int dy = Mathf.Abs(toCoord.Y - toCoord.Y);
		return dx + dy;
	}

	protected Vector2I _IdToCoord(long id)
	{
		int y = (int)(id / GridWidth);
		int x = (int)(id % GridWidth);
		return new Vector2I(x, y) + MapOrigin;
	}

}
