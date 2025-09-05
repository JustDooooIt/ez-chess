
using Godot;

public partial class MPGameManager : GameManager
{

	public override void _Ready()
	{
		base._Ready();
		GameState.Instance.IsSolo = false;
		GithubUtils.SubmitOperationOnInterval();
		var roomLabel = GetNode<Label>("CanvasLayer/Control/Room/Label");
		roomLabel.Text = GameState.Instance.RoomMetaData.Number.ToString();
	}

	private void OnExitGame()
	{
		_ = GithubUtils.ExitRoom(GameState.Instance.RoomMetaData.Id);
	}

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationExitTree:
			case NotificationWMCloseRequest:
			case NotificationCrash:
				OnExitGame();
				break;
		}
	}

}
