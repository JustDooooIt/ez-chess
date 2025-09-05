using Godot;

public partial class DisposeStateDecorator(IPieceState wrapped) : PieceStateDecorator(wrapped), IDisposable, IDisposeEventSender
{
  public void ReciveEvent(DisposeEvent @event)
  {
    PiecesManager.RemoveChild(PieceAdapter);
    PieceAdapter.QueueFree();
    if (!@event.recovered && PipelineAdapter is not OtherPipeline)
    {
      DisposeOperation operation = new()
      {
        PieceName = PieceAdapter.Name,
        Faction = PieceAdapter.Faction,
        Type = (int)OperationType.DISPOSE,
        CommentType = CommentType.GAME_DATA
      };
      GithubUtils.SaveOperation(GameState.Instance.RoomMetaData.Id, operation);
    }
    PiecesManager.Pieces.Remove(As<IPositionable>().MapPosition, PieceAdapter);
  }

  public void SendDisposeEvent()
  {
    ulong pieceId = GetPieceId();
    var piece = InstanceFromId(pieceId) as PieceAdapter;
    DisposeEvent @event = new(Faction, piece.Name);
    AddValve<DisposeEvent, DisposeStateValve>(@event);
  }
}