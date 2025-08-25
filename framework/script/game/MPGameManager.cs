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
	TreeExiting += OnTreeExisting;
	GameState.Instance.IsSolo = false;
  }

  private async void EnterRoom()
  {
	var roomNumber = GetNode<LineEdit>("CanvasLayer/Control/EnterRoom/RoomId");
	_roomMetaData = await GithubUtils.GetRoomInfo(roomNumber.Text.ToInt());
	GD.Print("Enter room");
	_player = await GithubUtils.EnterRoom(_roomMetaData.Id, GameState.Instance.PlayerFaction, PlayerType.PLAYER);
	GD.Print("Player ready");
	GD.Print("Waiting for other players");
	_others = await GithubUtils.WaitForOthers(_roomMetaData.Number, GameState.Instance.PlayerCount, _username);
	GD.Print("All players ready");
	GameState.Instance.Room = _roomMetaData;
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
	_roomMetaData = await GithubUtils.CreateGameRoom("normandy44");
	InitCypto();
	GD.Print("Create room success");
	_player = await GithubUtils.EnterRoom(_roomMetaData.Id, GameState.Instance.PlayerFaction, PlayerType.PLAYER);
	GD.Print("Game ready");
	GD.Print("Waiting for other players");
	var state = GetNode<Label>("CanvasLayer/Control/State/Label");
	state.Text = "Waiting";
	_waiterToken = new();
	_others = await GithubUtils.WaitForOthers(_roomMetaData.Number, GameState.Instance.PlayerCount, _username);
	state.Text = "Started";
	GD.Print("All players ready");
	GameState.Instance.Room = _roomMetaData;
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

  private async Task Polling()
  {
	await GithubUtils.ApplyOperation(_roomMetaData.Number, GameState.Instance.PlayerFaction, HashState(), (comment) =>
	{

	});
  }

  private string GetGithubToken()
  {
	var input = GetNode<LineEdit>("CanvasLayer/Control/Login/GithubTokenInput");
	return input.Text;
  }

  private void OnTreeExisting()
  {
	if (_player != null)
	{
	  _ = GithubUtils.LeaveRoom(_roomMetaData.Id, GameState.Instance.PlayerFaction, PlayerType.PLAYER);
	}
  }
}
