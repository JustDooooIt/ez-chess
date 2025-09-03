using Godot;

public partial class RenderEventHandler : BaseRenderEventHandler
{
  protected override InstanceValve CreateValve<T>(T @event)
  {
    return @event switch
    {
      MoveEvent moveEvent => new MoveInstanceValve((InstanceFromId(@event.pieceId) as PieceAdapter).Instance, moveEvent),
      PositionEvent positionEvent => new PositionInstanceValve((InstanceFromId(@event.pieceId) as PieceAdapter).Instance, positionEvent),
      AttackEvent attackEvent => new AttackInstanceValve((InstanceFromId(@event.pieceId) as PieceAdapter).Instance, attackEvent),
      RetreatEvent retreatEvent => new RetreatInstanceValve((InstanceFromId(@event.pieceId) as PieceAdapter).Instance, retreatEvent),
      _ => default,
    };  
  }
}