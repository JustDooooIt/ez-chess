using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// 轮询评论得到玩家操作并执行
/// </summary>
public interface IOperationRunner
{
  void RunOperation(GameManager manager, JsonObject operation, bool recovered);
}

/// <summary>
/// 设置环境变量, 比如当前是第几回合
/// </summary>
public interface IEnvironmentRunner
{
  void SetEnvironment(GameManager manager, JsonObject environment);
}