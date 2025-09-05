using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class AlameinManager : MPGameManager
{
	private Button _endStage;

	public override void _Ready()
	{
		base._Ready();
		_endStage = GetNode<Button>("CanvasLayer/Control/EndStage");
		_endStage.Pressed += GameState.Instance.EndStage;
		GameState.Instance.StageChanged += OnStageChanged;
		GameState.Instance.TurnChanged += OnTurnChanged;
		var gameloader = GetNode<GameLoader>("GameLoader");
		var TurnLabel = GetNode<Label>("CanvasLayer/Control/Turn/Label");
		TurnLabel.Text = "0";
		var stageLabel = GetNode<Label>("CanvasLayer/Control/Stage/Label");
		stageLabel.Text = "MOVE";
	}

	public void OnTurnChanged(int turn)
	{
		var TurnLabel = GetNode<Label>("CanvasLayer/Control/Turn/Label");
		TurnLabel.Text = turn.ToString();
	}

	private async void OnStageChanged(int stage)
	{
		var stageLabel = GetNode<Label>("CanvasLayer/Control/Stage/Label");
		if (stage == (int)GameState.Stages.MOVE)
		{
			// 当阵营是英军时,回合数+1
			stageLabel.Text = "MOVE";
			if (GameState.Instance.CurOperatorFaction == 1)
			{
				await GithubUtils.IncreaseTurn(GameState.Instance.RoomMetaData.Id);
			}
			GameState.Instance.CurOperatorFaction++;
			await GithubUtils.ChangeFaction(GameState.Instance.RoomMetaData.Id, GameState.Instance.CurOperatorFaction %= GameState.Instance.Factions.Length);
		}
		else if (stage == (int)GameState.Stages.ATTACK)
		{
			stageLabel.Text = "ATTACK";
		}
	}
}
