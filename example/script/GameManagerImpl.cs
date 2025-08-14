public partial class GameManagerImpl : GameManager
{
  public override void _Ready()
  {
    base._Ready();
    GameState.Instance.PlayerFaction = 1;
    GameState.Instance.CurOperator = 1;
  }
}