using System.Text.Json.Nodes;
using Godot;

public interface IOperationRunner
{
  void RunOperation(int opType, PieceAdapter piece, JsonObject gameData);
}