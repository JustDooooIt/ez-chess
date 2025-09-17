using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class GameConfig : Control
{
	[Export]
	public PackedScene Scene { get; set; }
	[Export]
	public string GameName { get; set; }
	[Export]
	public Array<string> Factions { get; set; }

	public override void _Ready()
	{
		if (GameState.Instance.Config.Count > 0)
		{
			GameName = GameState.Instance.Config["name"].AsString();
			Factions = GameState.Instance.Config["factions"].AsGodotArray<string>();
			Scene = GD.Load<PackedScene>(GameState.Instance.Config["path"].AsString());
		}
		FactionSelector();
	}

	private void FactionSelector()
	{
		var factionOption = GetNode<OptionButton>("HBoxContainer/VBoxContainer1/Faction");
		var createButton = GetNode<Button>("HBoxContainer/VBoxContainer1/Create");
		var enterButton = GetNode<Button>("HBoxContainer/VBoxContainer2/Enter");
		var token = GetNode<LineEdit>("HBoxContainer/VBoxContainer1/Token");
		var room = GetNode<LineEdit>("HBoxContainer/VBoxContainer2/Room");
		foreach (var faction in Factions)
		{
			factionOption.AddItem(faction);
		}
		enterButton.Pressed += async () =>
		{
			GameState.Instance.PlayerFaction = factionOption.Selected;
			GameState.Instance.Username = await GithubUtils.Login(token.Text);
			GameState.Instance.RoomMetaData = await GithubUtils.GetRoomInfo(room.Text.ToInt());
			await GithubUtils.EnterRoom(GameState.Instance.RoomMetaData.Id);
			await GithubUtils.ChooseFaction(GameState.Instance.RoomMetaData.Id, GameState.Instance.PlayerFaction);
			GetTree().ChangeSceneToPacked(Scene);
		};
		createButton.Pressed += async () =>
		{
			GameState.Instance.PlayerFaction = factionOption.Selected;
			GameState.Instance.Username = await GithubUtils.Login(token.Text);
			GameState.Instance.RoomMetaData = await GithubUtils.CreateRoom(GameName);
			await GithubUtils.EnterRoom(GameState.Instance.RoomMetaData.Id);
			await GithubUtils.ChooseFaction(GameState.Instance.RoomMetaData.Id, GameState.Instance.PlayerFaction);
			GetTree().ChangeSceneToPacked(Scene);
		};
	}

	//房主邀请其他玩家 然后玩家在github接受邀请
	public async Task Invite(string player)
	{
		await GithubUtils.InviteOthers(GameState.Instance.RoomMetaData.Id, player);
	}
}
