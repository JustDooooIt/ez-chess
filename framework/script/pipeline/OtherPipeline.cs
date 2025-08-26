using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;

public partial class OtherPipeline : PlayerPipeline
{
	public IOperationRunner OperationRunner { get; set; }

	public override void _Ready()
	{
		base._Ready();
	}

	public async void Polling()
	{
		while (true)
		{
			int faction = GetIndex();
			var hash = GameManager.HashState();
			await GithubUtils.ApplyOperation(GameState.Instance.RoomMetaData.Number, faction, hash, (gameData) =>
			{
				if (gameData.ContainsKey("operation") && gameData["operation"].AsObject().ContainsKey("type"))
				{
					var piece = GameManager.GetNode("Pieces").GetChild<PiecesManager>(faction).GetNode<PieceAdapter>(gameData["pieceName"].GetValue<string>());
					OperationRunner.RunOperation(gameData["operation"]["type"].GetValue<int>(), piece, gameData);
				}
			});
			await Task.Delay(2500);
		}
	}
}
