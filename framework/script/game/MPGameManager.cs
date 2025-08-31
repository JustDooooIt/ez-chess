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
	private RoomState _roomState;
	private UserData _player;
	private Dictionary<string, UserData> _others = [];
	private CancellationToken _waiterToken;

	public override void _Ready()
	{
		base._Ready();
		GameState.Instance.IsSolo = false;
	}

	// private async void EnterRoom()
	// {
	// 	await GithubUtils.EnterRoom(_roomMetaData.Id);
	// 	GD.Print("Enter room");
	// 	GD.Print("Player ready");
	// 	GD.Print("Waiting for other players");
	// 	_others = await GithubUtils.WaitForOthers(_roomMetaData.Number, GameState.Instance.PlayerCount, _username);
	// 	GD.Print("All players ready");
	// 	GameState.Instance.RoomMetaData = _roomMetaData;

	// 	foreach (var item in GetNode<Node>("Players").GetChildren())
	// 	{
	// 		if (item is OtherPipeline other)
	// 		{
	// 			other.Polling();
	// 		}
	// 	}
	// }

	// private async void CreateRoom()
	// {
	// 	_roomMetaData = await GithubUtils.CreateRoom(GameName, GameState.Instance.RoomState.Seats);
	// 	ListenRoomState();
	// 	GD.Print("Create room success");
	// 	await GithubUtils.EnterRoom(_roomMetaData.Id);
	// 	await GithubUtils.ChooseFaction(_roomMetaData.Id, GameState.Instance.PlayerFaction);
	// 	_roomState = await GithubUtils.GetRoomState(_roomMetaData.Number);
	// 	GD.Print("Game ready");
	// 	GD.Print("Waiting for other players");
	// 	var state = GetNode<Label>("CanvasLayer/Control/State/Label");
	// 	state.Text = "Waiting";
	// 	_waiterToken = new();
	// 	_others = await GithubUtils.WaitForOthers(_roomMetaData.Number, GameState.Instance.PlayerCount, _username);
	// 	state.Text = "Started";
	// 	GD.Print("All players ready");
	// 	GameState.Instance.RoomMetaData = _roomMetaData;
	// 	foreach (var item in GetNode<Node>("Players").GetChildren())
	// 	{
	// 		if (item is OtherPipeline other)
	// 		{
	// 			other.Polling();
	// 		}
	// 	}
	// }


	private void OnExitGame()
	{
		if (_player != null)
		{
			_ = GithubUtils.ExitRoom(_roomMetaData.Id);
		}
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
