using Godot;

public partial class AdvanceStateDecorator(IPieceState wrapped) : PieceStateDecorator<AdvanceEvent>(wrapped), IAdvancable, IAdvanceEventSender
{
  public void SendAdvanceEvent(Vector2I from, Vector2I to)
  {
    AddValve<AdvanceEvent, AdvanceStateValve>(new(Faction, PieceAdapter.Name, from, to, false));
  }

  protected override void DoReciveEvent(AdvanceEvent @event)
  {
    Query<IPositionable>().MapPosition = @event.to;
  }

  protected override void SaveOperation(AdvanceEvent @event)
  {
    var op = new AdvanceOperation()
    {
      From = @event.from,
      To = @event.to,
      PieceName = PieceAdapter.Name,
      Type = (int)OperationType.ADVANCE,
      Faction = PieceAdapter.Faction,
      CommentType = CommentType.GAME_DATA
    };
    GithubUtils.SaveOperation(GameState.Instance.RoomMetaData.Id, op);
  }
}