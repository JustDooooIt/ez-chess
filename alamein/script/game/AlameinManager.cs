using Godot;

public partial class AlameinManager : MPGameManager
{
  private Button _endStage;

  public override void _Ready()
  {
	base._Ready();
	_endStage = GetNode<Button>("CanvasLayer/Control/EndStage");
	_endStage.Pressed += GameState.Instance.EndStage;
  }


  private void OnStageChanged(int stage)
  {
	if (stage == (int)GameState.Stages.MOVE)
	{
	  GameState.Instance.Turn++;
	}
	else if (stage == (int)GameState.Stages.ATTACK)
	{

	}
  }

}
