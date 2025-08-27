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
		var loginBtn = GetNode<Button>("CanvasLayer/Control/Login/Login");
		var createBtn = GetNode<Button>("CanvasLayer/Control/CreateRoom");
		var enterBtn = GetNode<Button>("CanvasLayer/Control/EnterRoom/Enter");
		loginBtn.Pressed += Login;
		createBtn.Pressed += CreateRoom;
		enterBtn.Pressed += EnterRoom;
		GameState.Instance.IsSolo = false;
	}

	private async void EnterRoom()
	{
		var roomNumber = GetNode<LineEdit>("CanvasLayer/Control/EnterRoom/RoomId");
		_roomMetaData = await GithubUtils.GetRoomInfo(roomNumber.Text.ToInt());
		_roomState = await GithubUtils.GetRoomState(_roomMetaData.Number);
		ListenRoomState();
		if (_username == _roomState.Seats[GameState.Instance.PlayerFaction] || _roomState.Seats[GameState.Instance.PlayerFaction] == "")
		{
			await GithubUtils.EnterRoom(_roomMetaData.Id);
			_roomState.Seats[GameState.Instance.PlayerFaction] = _username;
			_roomState = await GithubUtils.UpdateRoomState(_roomMetaData.Id, _roomState);
			GD.Print("Enter room");
		}
		else
		{
			_player = null;
			GD.Print("The room is full.");
			return;
		}
		GD.Print("Player ready");
		GD.Print("Waiting for other players");
		_others = await GithubUtils.WaitForOthers(_roomMetaData.Number, GameState.Instance.PlayerCount, _username);
		GD.Print("All players ready");
		GameState.Instance.RoomMetaData = _roomMetaData;

		foreach (var item in GetNode<Node>("Players").GetChildren())
		{
			if (item is OtherPipeline other)
			{
				other.Polling();
			}
		}
	}

	private async void CreateRoom()
	{
		_roomMetaData = await GithubUtils.CreateRoom(GameName, GameState.Instance.RoomState.Seats);
		ListenRoomState();
		GD.Print("Create room success");
		await GithubUtils.EnterRoom(_roomMetaData.Id);
		_roomState = await GithubUtils.GetRoomState(_roomMetaData.Number);
		_roomState.Seats[GameState.Instance.PlayerFaction] = _username;
		_roomState = await GithubUtils.UpdateRoomState(_roomMetaData.Id, _roomState);
		GD.Print("Game ready");
		GD.Print("Waiting for other players");
		var state = GetNode<Label>("CanvasLayer/Control/State/Label");
		state.Text = "Waiting";
		_waiterToken = new();
		_others = await GithubUtils.WaitForOthers(_roomMetaData.Number, GameState.Instance.PlayerCount, _username);
		state.Text = "Started";
		GD.Print("All players ready");
		GameState.Instance.RoomMetaData = _roomMetaData;
		foreach (var item in GetNode<Node>("Players").GetChildren())
		{
			if (item is OtherPipeline other)
			{
				other.Polling();
			}
		}
	}

	private void InitCypto()
	{
		var rsa = _crypto.GenerateRsa(2048);
		if (!FileAccess.FileExists($"user://{_username}.key"))
		{
			rsa.Save($"user://{_username}.key");
		}
		if (!FileAccess.FileExists($"user://{_username}.pub"))
		{
			rsa.Save($"user://{_username}.pub", true);
		}
	}

	private async void Login()
	{
		string token = GetGithubToken();
		(_userId, _username) = await GithubUtils.Login(token);
		var popup = GetNode<Popup>("CanvasLayer/Control/Login/Tip");
		popup.Popup();
		var label = popup.GetNode<Label>("Label");
		if (_userId != "")
		{
			label.Text = "success";
			var tokenInput = GetNode<LineEdit>("CanvasLayer/Control/Login/GithubTokenInput");
			tokenInput.Editable = false;
			// var factionBtn = GetNode<OptionButton>("CanvasLayer/Control/Login/Faction");
			// GameState.Instance.PlayerFaction = factionBtn.Selected;
			GD.Print("Login success");
		}
		else
		{
			label.Text = "fail";
		}
	}

	private string GetGithubToken()
	{
		var input = GetNode<LineEdit>("CanvasLayer/Control/Login/GithubTokenInput");
		return input.Text;
	}

	private void OnExitGame()
	{
		if (_player != null)
		{
			_ = GithubUtils.LeaveRoom(_roomMetaData.Id, GameState.Instance.PlayerFaction, UserType.PLAYER);
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

	public async void ListenRoomState()
	{
		while (true)
		{
			try
			{
				_roomState = await GithubUtils.GetRoomState(_roomMetaData.Number);
				await Task.Delay(2500);
			}
			catch (System.Exception)
			{
			}
		}
	}
}
