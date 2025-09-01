using Godot;

public partial class RenderEventHandler : BaseRenderEventHandler
{
  protected override InstanceValve CreateValve<T>(T @event)
  {
    return @event switch
    {
      MoveEvent moveEvent => new MoveInstanceValve((IPieceInstance)InstanceFromId(@event.pieceId), moveEvent),
      PositionEvent positionEvent => new PositionInstanceValve((IPieceInstance)InstanceFromId(@event.pieceId), positionEvent),
      _ => default,
    };
  }
}