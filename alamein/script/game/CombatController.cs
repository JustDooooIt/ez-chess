using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CombatController : RefCounted
{
	public readonly static CombatController Instance = new();
	public CombatResult[][] CombatTable { get; } = [
	[CombatResult.AE, CombatResult.AR, CombatResult.AR, CombatResult.AR, CombatResult.DR1, CombatResult.DR3, CombatResult.DE],
	[CombatResult.AR, CombatResult.AR, CombatResult.AR, CombatResult.DR1, CombatResult.DR2, CombatResult.DR4, CombatResult.DE],
	[CombatResult.AR, CombatResult.AR, CombatResult.DR1, CombatResult.DR2, CombatResult.DR3, CombatResult.DE, CombatResult.DE],
	[CombatResult.AR, CombatResult.DR1, CombatResult.DR2, CombatResult.DR3, CombatResult.DR4, CombatResult.DE, CombatResult.DE],
	[CombatResult.AR, CombatResult.DR2, CombatResult.DR3, CombatResult.DR4, CombatResult.DE, CombatResult.DE, CombatResult.DE],
	[CombatResult.DR1, CombatResult.DR3, CombatResult.DR4, CombatResult.DE, CombatResult.DE, CombatResult.DE, CombatResult.DE],
	];

	public Dictionary<ulong, HashSet<ulong>> Combats { get; } = [];

	public static int AttackPointsToIndex(float a1, float a2)
	{
		var ratio = Mathf.Floor(a1 / a2);
		if (ratio <= 0.5)
		{
			return 0;
		}
		else
		{
			return Mathf.FloorToInt(ratio);
		}
	}

	public void AddCombatUnit(ulong from, ulong target)
	{
		if (Combats.TryGetValue(target, out var froms))
		{
			froms.Add(from);
		}
		else
		{
			Combats.Add(target, [from]);
		}
	}

	public CombatResult ProcessCombat(ulong target)
	{
		float fromAttackPoint = Combats[target].Select(GetAttackPoint).Sum();
		float targetAttackPoint = GetAttackPoint(target);
		int x = AttackPointsToIndex(fromAttackPoint, targetAttackPoint);
		int y = GD.RandRange(0, 5);
		var combatResult = CombatTable[y][x];
		var piece = InstanceFromId(target) as PieceAdapter;
		switch (combatResult)
		{
			case CombatResult.DR1:
				{
					if (piece is GeneralPiece generalPiece)
						generalPiece.Retreatable = true;
					piece.State.Query<IRetreatRangeProvider>().RetreatRange = 1;
				}
				break;
			case CombatResult.DR2:
				{
					if (piece is GeneralPiece generalPiece)
						generalPiece.Retreatable = true;
					piece.State.Query<IRetreatRangeProvider>().RetreatRange = 2;
				}
				break;
			case CombatResult.DR3:
				{
					if (piece is GeneralPiece generalPiece)
						generalPiece.Retreatable = true;
					piece.State.Query<IRetreatRangeProvider>().RetreatRange = 3;
				}
				break;
			case CombatResult.DR4:
				{
					if (piece is GeneralPiece generalPiece)
						generalPiece.Retreatable = true;
					piece.State.Query<IRetreatRangeProvider>().RetreatRange = 4;
				}
				break;
			default:
				break;
		}
		GD.Print("combatResult: ", new Vector2I(x, y));
		Combats.Remove(target);
		return combatResult;
	}

	private float GetAttackPoint(ulong piece)
	{
		var pieceAdapter = InstanceFromId(piece) as PieceAdapter;
		return pieceAdapter.State.Query<IAttackPointProvider>().AttackPoint;
	}
}


public enum CombatResult
{
	// AE为攻击方被消灭
	AE,
	// AR为攻击方撤退
	AR,
	// DR为防御方撤退
	DR1,
	DR2,
	DR3,
	DR4,
	// DE为防御方被消灭
	DE
}
