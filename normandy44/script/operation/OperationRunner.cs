
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
        var data = GithubUtils.Deserialize<GameData>(gameData);
        var moveOperation = data.Operation as MoveOperation;
        piece.State.As<IMoveEventSender>()?.SendMoveEvent(moveOperation.From, moveOperation.To, true);
        break;
    }
  }
}