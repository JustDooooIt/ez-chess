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
			await GithubUtils.ApplyOperations(GameState.Instance.RoomMetaData.Number, (operation) =>
			{
				if (operation.ContainsKey("type"))
				{
					OperationRunner.RunOperation(GameManager, operation, true);
				}
			});
		}
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
				await GithubUtils.ApplyOperation(GameState.Instance.RoomMetaData.Number, faction, (operation) =>
				{
					if (operation.ContainsKey("type"))
					{
						OperationRunner.RunOperation(GameManager, operation, false);
					}
				});
				await Task.Delay(2500);
			}
		}
	}
}
