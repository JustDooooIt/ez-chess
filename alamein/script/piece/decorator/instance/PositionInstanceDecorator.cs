using Godot;

[RegisterValve("Position", ValveTypes.INSTANCE)]
public partial class PositionInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator<PositionEvent>(wrapped), IPositionable
{
  private Vector2I _mapPosition;

  public Vector2I MapPosition { get => GetMapPosition(); set => SetMapPosition(value); }

  private void SetMapPosition(Vector2I position)
  {
    _mapPosition = position;
    HexMap.PlacePiece(PieceAdapter, position);
  }

  private Vector2I GetMapPosition()
  {
    return _mapPosition;
  }

  protected override void _ReciveEvent(PositionEvent @event)
  {
    MapPosition = @event.position;
  }
}