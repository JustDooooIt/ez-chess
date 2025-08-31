using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public partial class MPGameManager : GameManager
{
	private Crypto _crypto = new();
	private string _username;
	private string _userId;
	private RoomMetaData _roomMetaData;
	private Dictionary<string, UserData> _others = [];
	private CancellationToken _waiterToken;

	public override void _Ready()
	{
		base._Ready();
		GameState.Instance.IsSolo = false;
	}

	private void OnExitGame()
	{
		_ = GithubUtils.ExitRoom(_roomMetaData.Id);
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
