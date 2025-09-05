using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;

public partial class EnvironmentRunner : RefCounted, IEnvironmentRunner
{
  public void SetEnvironment(GameManager manager, JsonObject environmentJson)
  {
    if (environmentJson.ContainsKey("Type") && environmentJson["Type"].GetValue<int>() == (int)EnvDataType.FACTION)
    {
      var env = GithubUtils.Deserialize<FactionData>(environmentJson);
      GameState.Instance.CurOperatorFaction = env.Faction;
    }
    else if (environmentJson["Type"].GetValue<int>() == (int)EnvDataType.TURN)
    {
      GameState.Instance.Turn++;
    }
  }
}