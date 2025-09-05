using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class MoveStateDecorator(IPieceState piece, float movement) :
  PieceStateDecorator(piece), IMoveable, IMoveEventSender, IResetable
{
  public float Movement { get; set; } = movement;
  public float ResidualMovement { get; set; } = movement;

  public void ReciveEvent(MoveEvent @event)
  {
    As<IPositionable>().MapPosition = @event.to;
    ResidualMovement -= @event.cost;
    if (!@event.recovered && PipelineAdapter is not OtherPipeline)
    {
      var op = new MoveOperation()
      {
        From = @event.from,
        To = @event.to,
        Path = @event.path,
        PieceName = PieceAdapter.Name,
        Type = (int)OperationType.MOVE,
        Faction = PieceAdapter.Faction,
        CommentType = CommentType.GAME_DATA
      };
      GithubUtils.SaveOperation(GameState.Instance.RoomMetaData.Id, op);
    }
    PiecesManager.Pieces.Move(@event.from, @event.to, PieceAdapter);
  }

  public void SendMoveEvent(Vector2I from, Vector2I to, bool recovered = false)
  {
    ulong pieceId = GetPieceId();
    var piece = InstanceFromId(pieceId) as PieceAdapter;
    var path = piece.Instance.HexMap.FindPath(from, to, ResidualMovement, out var cost);
    if (path.Length > 0)
    {
      AddValve<MoveEvent, MoveStateValve>(new(PieceAdapter.Faction, PieceAdapter.Name, from, to, cost, path, recovered));
    }
  }

  public override V As<V>() where V : class
  {
    if (typeof(V) == typeof(IMoveable))
      return this as V;

    return base.As<V>();
  }

  public void ReciveEvent(ResetEvent @event)
  {
    ResidualMovement = Movement;
  }

}
