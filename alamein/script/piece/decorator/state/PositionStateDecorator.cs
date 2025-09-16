using Godot;

public partial class PositionStateDecorator(IPieceState wrapped) : PieceStateDecorator<PositionEvent>(wrapped), IPositionable, IPositionEventSender
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
    var valve = new PositionStateValve(this, new(PieceAdapter.Faction, PieceAdapter.Name, position));
    PipelineAdapter.StatePipeline.AddValve(valve);
    PipelineAdapter.RenderPipeline.RegisterValve<PositionEvent>(valve);
  }
 
  protected override void _ReciveEvent(PositionEvent @event)
  {
    
  }

  protected override Operation _ToOperation(PositionEvent @event)
  {
    return null;
  }
}