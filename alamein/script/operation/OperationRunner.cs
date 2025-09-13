
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;

public partial class OperationRunner : RefCounted, IOperationRunner
{
	public void RunOperation(GameManager manager, JsonObject data, bool recovered)
	{
		int opType = data["type"].GetValue<int>();
		switch ((OperationType)opType)
		{
			case OperationType.MOVE:
				{
					var operation = GithubUtils.Deserialize<MoveOperation>(data);
					var piece = manager.GetNode<Node>("Pieces").GetChild<Node>(operation.Faction).GetNode<PieceAdapter>(operation.PieceName);
					piece.State.Query<IMoveEventSender>()?.SendMoveEvent(operation.From, operation.To, recovered);
				}
				break;
			case OperationType.ATTACK:
				{
					var operation = GithubUtils.Deserialize<AttackOperation>(data);
					var fromPiece = PieceAdapter.GetPiece(manager, operation.FromFaction, operation.From);
					var targetPiece = PieceAdapter.GetPiece(manager, operation.TargetFaction, operation.Target);
					var combatResult = operation.CombatResult;
					switch (combatResult)
					{
						case (int)CombatResult.AE:
							{
								fromPiece.State.Query<IDisposeEventSender>().SendDisposeEvent();
							}
							break;
						case (int)CombatResult.AR:
							{
								if (fromPiece is GeneralPiece generalPiece)
									generalPiece.Retreatable = true;
								fromPiece.State.Query<IRetreatRangeProvider>().RetreatRange = 1;
							}
							break;
						case (int)CombatResult.DR1:
							{
								if (targetPiece is GeneralPiece generalPiece)
									generalPiece.Retreatable = true;
								targetPiece.State.Query<IRetreatRangeProvider>().RetreatRange = 1;
							}
							break;
						case (int)CombatResult.DR2:
							{
								if (targetPiece is GeneralPiece generalPiece)
									generalPiece.Retreatable = true;
								targetPiece.State.Query<IRetreatRangeProvider>().RetreatRange = 2;
							}
							break;
						case (int)CombatResult.DR3:
							{
								if (targetPiece is GeneralPiece generalPiece)
									generalPiece.Retreatable = true;
								targetPiece.State.Query<IRetreatRangeProvider>().RetreatRange = 3;
							}
							break;
						case (int)CombatResult.DR4:
							{
								if (targetPiece is GeneralPiece generalPiece)
									generalPiece.Retreatable = true;
								targetPiece.State.Query<IRetreatRangeProvider>().RetreatRange = 4;
							}
							break;
						case (int)CombatResult.DE:
							{
								targetPiece.State.Query<IDisposeEventSender>().SendDisposeEvent();
							}
							break;
						default:
							break;
					}

				}
				break;
			case OperationType.RETREAT:
				{
					var operation = GithubUtils.Deserialize<RetreatOperation>(data);
					var piece = PieceAdapter.GetPiece(manager, operation.Faction, operation.PieceName);
					piece.State.Query<IRetreatEventSender>()?.SendRetreatEvent(operation.From, operation.To);
				}
				break;
			// case OperationType.DISPOSE:
			// 	{
			// 		var operation = GithubUtils.Deserialize<DisposeOperation>(data);
			// 		var piece = PieceAdapter.GetPiece(manager, operation.Faction, operation.PieceName);
			// 		piece.State.Query<IDisposeEventSender>().SendDisposeEvent();
			// 	}
			// 	break; 
			case OperationType.ADVANCE:
				{
					var operation = GithubUtils.Deserialize<AdvanceOperation>(data);
					var piece = PieceAdapter.GetPiece(manager, operation.Faction, operation.PieceName);
					piece.State.Query<IAdvanceEventSender>().SendAdvanceEvent(operation.From, operation.To);
				}
				break;
		}
	}
}
