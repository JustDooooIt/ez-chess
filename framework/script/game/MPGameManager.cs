public partial class MPGameManager : GameManager
{

	public override void _Ready()
	{
		base._Ready();
		GameState.Instance.IsSolo = false;
		GithubUtils.SubmitOperationOnInterval();
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
