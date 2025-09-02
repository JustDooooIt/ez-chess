using System.Linq;
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
	gameLoader.GameLoaded += Polling;
  }

  private async void Recover()
  {
	foreach (var child in GetChildren())
	{
	  await GithubUtils.ApplyOperations(GameState.Instance.RoomMetaData.Number, (gameData) =>
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
	GameState.Instance.IsRecovered = true;
	EmitSignal("BoardLoaded");
  }

  private async void Polling()
  {
	await ToSignal(this, "BoardLoaded");
	foreach (var pipeline in GetChildren().Where(e => e is OtherPipeline).Cast<OtherPipeline>())
	{
	  while (true)
	  {
		int faction = pipeline.GetIndex();
		await GithubUtils.ApplyOperation(GameState.Instance.RoomMetaData.Number, faction, (gameData) =>
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
}
