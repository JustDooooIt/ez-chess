using Godot;

public partial class PositionInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator(wrapped), IPositionable
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

  public void SendMoveEvent(Vector2I position)
  {
    var valve = new PositionInstanceValve(this, new(Wrapped.GetInstanceId(), position));
    PipelineAdapter.RenderPipeline.AddValve(valve);
  }
}