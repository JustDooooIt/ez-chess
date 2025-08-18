using Godot;

public partial class PositionStateDecorator(IPieceState wrapped) : PieceStateDecorator(wrapped), IPositionable, IPositionEventSender
{
  private Vector2I _mapPosition;

  public Vector2I MapPosition { get => GetMapPosition(); set => SetMapPosition(value); }

  private void SetMapPosition(Vector2I position)
  {
    _mapPosition = position;
  }

  private Vector2I GetMapPosition()
  {
    return _mapPosition;
  }

  public void SendPositionEvent(Vector2I position)
  {
    ulong instance = GetPieceInstanceId();
    var valve = new PositionStateValve(this, new(instance, position));
    PipelineAdapter.StatePipeline.AddValve(valve);
    PipelineAdapter.RenderPipeline.RegisterValve<RenderPositionEvent>(valve);
  }
}