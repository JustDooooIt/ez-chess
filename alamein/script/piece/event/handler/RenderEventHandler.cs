using Godot;

public partial class RenderEventHandler : BaseRenderEventHandler
{
  protected override InstanceValve CreateValve<T>(T @event)
  {
    return @event switch
    {
      MoveEvent moveEvent => new MoveInstanceValve(GetPiece(moveEvent.faction, moveEvent.pieceName).Instance, moveEvent),
      PositionEvent positionEvent => new PositionInstanceValve(GetPiece(positionEvent.faction, positionEvent.pieceName).Instance, positionEvent),
      AttackEvent attackEvent => new AttackInstanceValve(GetPiece(attackEvent.fromFaction, attackEvent.fromPiece).Instance, attackEvent),
      RetreatEvent retreatEvent => new RetreatInstanceValve(GetPiece(retreatEvent.faction, retreatEvent.pieceName).Instance, retreatEvent),
      _ => default,
    };
  }

  private PieceAdapter GetPiece(int faction, string pieceName)
  {
    return GameManager.GetNode<Node>("Pieces").GetChild(faction).GetNode<PieceAdapter>(pieceName);
  }
}