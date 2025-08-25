using Godot;
using System;

public partial class GameConfig : Control
{
  [Export]
  public string ScenePath { get; set; }

  public override void _Ready()
  {
	FactionSelector();
  }

  private void FactionSelector()
  {
	var factionOption = GetNode<OptionButton>("VBoxContainer/OptionButton");
	var confirmButton = GetNode<Button>("VBoxContainer/Button");
	confirmButton.Pressed += () =>
	{
	  GameState.Instance.PlayerFaction = factionOption.Selected;
	  GetTree().ChangeSceneToFile(ScenePath);
	};
  }
}
