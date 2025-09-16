using Godot;

public partial class AdvanceStateDecorator(IPieceState wrapped) : PieceStateDecorator<AdvanceEvent>(wrapped), IAdvancable, IAdvanceEventSender
{
  public void SendAdvanceEvent(Vector2I from, Vector2I to)
  {
    AddValve<AdvanceEvent, AdvanceStateValve>(new(Faction, PieceAdapter.Name, from, to, false));
  }

  protected override void _ReciveEvent(AdvanceEvent @event)
  {
    Query<IPositionable>().MapPosition = @event.to;
  }

  protected override Operation _ToOperation(AdvanceEvent @event)
  {
    return new AdvanceOperation()
    {
      From = @event.from,
      To = @event.to,
      PieceName = PieceAdapter.Name,
      Type = (int)OperationType.ADVANCE,
      Faction = PieceAdapter.Faction,
      CommentType = CommentType.GAME_DATA
    };
  }
}