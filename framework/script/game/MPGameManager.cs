using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public partial class MPGameManager : GameManager
{
  private Crypto _crypto = new();
  private string _username;
  private GithubUtils.RoomMetaData _roomMetaData;
  private Dictionary<string, GithubUtils.UserData> _players = [];
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
  }

  private async void EnterRoom()
  {
    var roomNumber = GetNode<LineEdit>("CanvasLayer/Control/EnterRoom/RoomId");
    _roomMetaData = await GithubUtils.EnterRoom(roomNumber.Text.ToInt());
    GD.Print("Enter room");
    await GameReady(_roomMetaData.Id, _username, GameState.Instance.PlayerFaction);
    GD.Print("Player ready");
    GD.Print("Waiting for other players");
    _players = await GithubUtils.WaitOthers(_roomMetaData.Number, GameState.Instance.PlayerCount);
    GD.Print("All players ready");
  }

  private async void CreateRoom()
  {
    _roomMetaData = await GithubUtils.CreateGameRoom("normandy44");
    GD.Print("Create room success");
    if (await GameReady(_roomMetaData.Id, _username, GameState.Instance.PlayerFaction))
    {
      GD.Print("Game ready");
    }
    GD.Print("Waiting for other players");
    var state = GetNode<Label>("CanvasLayer/Control/State/Label");
    state.Text = "Waiting";
    _waiterToken = new();
    _players = await GithubUtils.WaitOthers(_roomMetaData.Number, GameState.Instance.PlayerCount, _waiterToken);
    state.Text = "Started";
    GD.Print("All players ready");
  }

  private async Task<bool> GameReady(string roomId, string username, int faction)
  {
    InitCypto();
    return await GithubUtils.GameReady(roomId, username, faction);
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
    _username = await GithubUtils.Login(token);
    var popup = GetNode<Popup>("CanvasLayer/Control/Login/Tip");
    popup.Popup();
    var label = popup.GetNode<Label>("Label");
    if (_username != "")
    {
      label.Text = "success";
      var tokenInput = GetNode<LineEdit>("CanvasLayer/Control/Login/GithubTokenInput");
      tokenInput.Editable = false;
      var factionBtn = GetNode<OptionButton>("CanvasLayer/Control/Login/Faction");
      GameState.Instance.PlayerFaction = factionBtn.Selected;
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

}
