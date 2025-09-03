using Godot;

public partial class GeneralPiece : PieceAdapter
{
  public bool Retreatable { get; set; } = false;

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseButton)
    {
      if (GameState.Instance.Stage == (int)GameState.Stages.MOVE &&
          Instance.IsSelected &&
          !Instance.IsHover &&
          !Instance.IsRunning &&
          !Retreatable &&
          Faction == GameState.Instance.CurOperatorFaction &&
          Faction == GameState.Instance.PlayerFaction &&
          GameState.Instance.HoveredPiece == null &&
          mouseButton.ButtonMask == MouseButtonMask.Right)
      {
        var from = (Instance.Origin as PieceInstance).Position;
        var to = (Instance.Origin as PieceInstance).GetGlobalMousePosition();
        Move(from, to);
      }
      else if (GameState.Instance.Stage == (int)GameState.Stages.ATTACK &&
              Instance.IsSelected &&
              !Instance.IsHover &&
              !Instance.IsRunning &&
              Faction == GameState.Instance.CurOperatorFaction &&
              Faction == GameState.Instance.PlayerFaction &&
              GameState.Instance.HoveredPiece != null &&
              GameState.Instance.HoveredPiece.Faction != GameState.Instance.PlayerFaction &&
              mouseButton.ButtonMask == MouseButtonMask.Right)
      {
        Attack(GameState.Instance.HoveredPiece);
      }
      else if (Instance.IsSelected &&
               !Instance.IsHover &&
               !Instance.IsRunning &&
               Retreatable &&
               Faction == GameState.Instance.PlayerFaction)
      {
        var from = (Instance.Origin as PieceInstance).Position;
        var to = (Instance.Origin as PieceInstance).GetGlobalMousePosition();
        Retreat(from, to);
      }
    }
  }

  private void Attack(PieceAdapter targetPiece)
  {
    State.As<IAttackEventSender>()?.SendAttackEvent(targetPiece.State.As<IPositionable>().MapPosition, targetPiece);
  }

  private void Move(Vector2 from, Vector2 to)
  {
    var _from = Instance.HexMap.ToMapPosition(from);
    var _to = Instance.HexMap.ToMapPosition(to);
    State.As<IMoveEventSender>()?.SendMoveEvent(_from, _to);
  }

  private void Retreat(Vector2 from, Vector2 to)
  {
    var _from = Instance.HexMap.ToMapPosition(from);
    var _to = Instance.HexMap.ToMapPosition(to);
    State.As<IRetreatEventSender>()?.SendRetreatEvent(_from, _to);
  }
}