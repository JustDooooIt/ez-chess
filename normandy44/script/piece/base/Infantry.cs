using Godot;

public partial class Infantry : PieceAdapter
{
  public override void _Ready()
  {
    base._Ready();

  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseButton)
    {
      if (Instance.IsSelected &&
          !Instance.IsHover &&
          Faction == GameState.Instance.PlayerFaction &&
          mouseButton.ButtonMask == MouseButtonMask.Right)
      {
        var from = (Instance.Origin as PieceInstance).Position;
        var to = (Instance.Origin as PieceInstance).GetGlobalMousePosition();
        Move(from, to);
      }
    }
  }

  private void Move(Vector2 from, Vector2 to)
  {
    var _from = Instance.HexMap.ToMapPosition(from);
    var _to = Instance.HexMap.ToMapPosition(to);
    State.As<IMoveEventSender>()?.SendMoveEvent(_from, _to);
  }
}