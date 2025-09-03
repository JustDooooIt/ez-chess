
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
					piece.State.As<IMoveEventSender>()?.SendMoveEvent(operation.From, operation.To, recovered);
				}
				break;
			case OperationType.ATTACK:
				{
					var operation = GithubUtils.Deserialize<AttackOperation>(data);
					GameState.Instance.CurOperatorFaction = GameState.Instance.PlayerFaction;
					var piece = GetPiece(manager, operation.TargetFaction, operation.Target);
					if (piece is GeneralPiece generalPiece)
						generalPiece.Retreatable = true;
				}
				break;
			case OperationType.RETREAT:
				{
					var operation = GithubUtils.Deserialize<RetreatOperation>(data);
					Vector2I[] path = operation.Path;
					var piece = GetPiece(manager, operation.Faction, operation.PieceName);
					var tween = piece.CreateTween();
					foreach (var point in path)
					{
						tween.TweenProperty(piece.Instance.Origin as Node2D, "position", piece.HexMap.ToLocalPosition(point), 1);
					}
					
				}
				break;
		}
	}

	private static PieceAdapter GetPiece(GameManager manager, int faction, string pieceName)
	{
		return manager.GetNode<Node>("Pieces").GetChild<Node>(faction).GetNode<PieceAdapter>(pieceName);
	}
}
