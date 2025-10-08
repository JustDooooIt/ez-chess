using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

[RegisterValve("Move", ValveTypes.STATE, true)]
public partial class MoveStateDecorator(IPieceState piece, float movement) :
	PieceStateDecorator<MoveEvent>(piece), IMovable, IMoveEventSender, IResetable
{
	public float Movement { get; set; } = movement;
	public float ResidualMovement { get; set; } = movement;

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

	public override V Query<V>() where V : class
	{
		if (typeof(V) == typeof(IMovable))
			return this as V;

		return base.Query<V>();
	}

	public void ReciveEvent(ResetEvent @event)
	{
		ResidualMovement = Movement;
	}

	protected override void _ReciveEvent(MoveEvent @event)
	{
		Query<IPositionable>().MapPosition = @event.to;
		ResidualMovement -= @event.cost;
		PiecesManager.Pieces.Move(@event.from, @event.to, PieceAdapter);
	}

	protected override Operation _ToOperation(MoveEvent @event)
	{
		return new MoveOperation()
		{
			From = @event.from,
			To = @event.to,
			Path = @event.path,
			PieceName = PieceAdapter.Name,
			Type = (int)OperationType.MOVE,
			Faction = PieceAdapter.Faction,
			CommentType = CommentType.GAME_DATA
		};
	}
}
