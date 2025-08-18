using Godot;

public partial class RenderEventHandler : BaseRenderEventHandler
{
  protected override InstanceValve CreateValve<T>(T @event)
  {
    return @event switch
    {
      RenderMoveEvent moveEvent => new MoveInstanceValve((IPieceInstance)InstanceFromId(@event.pieceId), moveEvent),
      RenderPositionEvent positionEvent => new PositionInstanceValve((IPieceInstance)InstanceFromId(@event.pieceId), positionEvent),
      _ => default,
    };
  }
}