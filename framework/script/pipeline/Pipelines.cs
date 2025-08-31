using System.Threading.Tasks;
using Godot;

public partial class Pipelines : Node
{
  [Signal]
  public delegate void BoardLoadedEventHandler();

  private GameLoader gameLoader;

  private GameManager GameManager { get; set; }
  public IOperationRunner OperationRunner { get; set; }

  public override void _Ready()
  {
    GameManager = GetNode<GameManager>("/root/Game");
    gameLoader = GetNode<GameLoader>("/root/Game/GameLoader");
    gameLoader.GameLoaded += Recover;
    gameLoader.GameLoaded += StartOthersPolling;
  }

  private async void Recover()
  {
    foreach (var child in GetChildren())
    {
      if (child is PlayerPipeline player)
      {
        var hash = GameManager.HashState();
        await GithubUtils.ApplyOperation(GameState.Instance.RoomMetaData.Number, hash, (gameData) =>
        {
          if (gameData.ContainsKey("operation") && gameData["operation"].AsObject().ContainsKey("type"))
          {
            var piece = GameManager.GetNode("Pieces")
              .GetChild<PiecesManager>(gameData["faction"].GetValue<int>())
              .GetNode<PieceAdapter>(gameData["pieceName"].GetValue<string>());
            OperationRunner.RunOperation(gameData["operation"]["type"].GetValue<int>(), piece, gameData);
          }
        });
      }
    }
    EmitSignal("BoardLoaded");
  }


  private async void StartOthersPolling()
  {
    await ToSignal(this, "BoardLoaded");
    foreach (var item in GetChildren())
    {
      if (item is OtherPipeline other)
      {
        Polling();
      }
    }
  }

  private async void Polling()
  {
    while (true)
    {
      int faction = GetIndex();
      var hash = GameManager.HashState();
      await GithubUtils.ApplyOperation(GameState.Instance.RoomMetaData.Number, faction, hash, (gameData) =>
      {
        if (gameData.ContainsKey("operation") && gameData["operation"].AsObject().ContainsKey("type"))
        {
          var piece = GameManager.GetNode("Pieces")
            .GetChild<PiecesManager>(faction)
            .GetNode<PieceAdapter>(gameData["pieceName"].GetValue<string>());
          OperationRunner.RunOperation(gameData["operation"]["type"].GetValue<int>(), piece, gameData);
        }
      });
      await Task.Delay(2500);
    }
  }
}