using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class MoveStateDecorator(IPieceState piece, float movement) :
  PieceStateDecorator(piece), IMoveable, IMoveEventSender
{
  public float CurMovement { get; set; } = movement;
  public float ResidualMovement { get; set; } = movement;

  public void ReciveEvent(MoveEvent @event)
  {
    As<IPositionable>().MapPosition = @event.to;
    if (!@event.recovered && PipelineAdapter is not OtherPipeline)
    {
      var op = new MoveOperation()
      {
        From = @event.from,
        To = @event.to,
        Path = @event.path,
        PieceName = PieceAdapter.Name,
        Type = (int) OperationType.MOVE,
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
    var path = piece.Instance.HexMap.FindPath(from, to, ResidualMovement);
    if (path.Length > 0)
    {
      // Valve valve = new MoveStateValve(this, new(pieceId, from, to, path, recovered));
      // PipelineAdapter.StatePipeline.AddValve(valve);
      // PipelineAdapter.RenderPipeline.RegisterValve<MoveEvent>(valve);
      AddValve<MoveEvent, MoveStateValve>(new(pieceId, from, to, path, recovered));
    }
  }

  public override V As<V>() where V : class
  {
    if (typeof(V) == typeof(IMoveable))
      return this as V;

    return base.As<V>();
  }
}
