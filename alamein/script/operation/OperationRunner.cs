
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;

public partial class OperationRunner : RefCounted, IOperationRunner
{
	public void RunOperation(int opType, PieceAdapter piece, JsonObject gameData)
	{
		switch ((OperationType)opType)
		{
			case OperationType.MOVE:
				var data = GithubUtils.Deserialize<GameData<MoveOperation>>(gameData);
				piece.State.As<IMoveEventSender>()?.SendMoveEvent(data.Operation.From, data.Operation.To, true);
				break;
		}
	}
}
