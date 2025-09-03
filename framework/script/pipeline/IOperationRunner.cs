using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;

public interface IOperationRunner
{
  void RunOperation(GameManager manager, JsonObject operation, bool recovered);
}