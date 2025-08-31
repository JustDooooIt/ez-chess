using Godot;
using System;
using System.Threading.Tasks;

public partial class GameConfig : Control
{
  [Export]
  public string ScenePath { get; set; }
  [Export]
  public string GameName { get; set; }

  public override void _Ready()
  {
	FactionSelector();
  }

  private void FactionSelector()
  {
	var factionOption = GetNode<OptionButton>("HBoxContainer/VBoxContainer/OptionButton");
	var confirmButton = GetNode<Button>("HBoxContainer/VBoxContainer/Finish");
	var createBtn = GetNode<Button>("HBoxContainer/VBoxContainer/Create");
	var token = GetNode<LineEdit>("HBoxContainer/VBoxContainer/Token");
	var room = GetNode<LineEdit>("HBoxContainer/VBoxContainer/Room");
	var invitedPlayer = GetNode<LineEdit>("HBoxContainer/VBoxContainer2/Player");
	var inviteBtn = GetNode<Button>("HBoxContainer/VBoxContainer2/Invite");
	var enterBtn = GetNode<Button>("HBoxContainer/VBoxContainer/Enter");
	confirmButton.Pressed += async () =>
	{
	  GameState.Instance.PlayerFaction = factionOption.Selected;
	  GameState.Instance.Username = await GithubUtils.Login(token.Text);
	  GameState.Instance.RoomMetaData = await GithubUtils.GetRoomInfo(room.Text.ToInt());
	  await GithubUtils.EnterRoom(GameState.Instance.RoomMetaData.Id);
	  await GithubUtils.ChooseFaction(GameState.Instance.RoomMetaData.Id, GameState.Instance.PlayerFaction);
	};
	createBtn.Pressed += async () =>
	{
	  GameState.Instance.PlayerFaction = factionOption.Selected;
	  GameState.Instance.Username = await GithubUtils.Login(token.Text);
	  GameState.Instance.RoomMetaData = await GithubUtils.CreateRoom(GameName);
	  await GithubUtils.EnterRoom(GameState.Instance.RoomMetaData.Id);
	  await GithubUtils.ChooseFaction(GameState.Instance.RoomMetaData.Id, GameState.Instance.PlayerFaction);
	};
	inviteBtn.Pressed += async () =>
	{
	  var player = invitedPlayer.Text;
	  await Invite(player);
	};
	enterBtn.Pressed += () => { GetTree().ChangeSceneToFile(ScenePath); };
  }

  //房主邀请其他玩家 然后玩家在github接受邀请
  public async Task Invite(string player)
  {
	await GithubUtils.InviteOthers(GameState.Instance.RoomMetaData.Id, player);
  }
}
