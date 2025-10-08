using Godot;

[RegisterValve("Dispose", ValveTypes.STATE, true)]
public partial class DisposeStateDecorator(IPieceState wrapped) : PieceStateDecorator<DisposeEvent>(wrapped), IDisposable, IDisposeEventSender
{

	public void SendDisposeEvent()
	{
		ulong pieceId = GetPieceId();
		var piece = InstanceFromId(pieceId) as PieceAdapter;
		DisposeEvent @event = new(Faction, piece.Name);
		AddValve<DisposeEvent, DisposeStateValve>(@event);
	}

	protected override void _ReciveEvent(DisposeEvent @event)
	{
		PiecesManager.RemoveChild(PieceAdapter);
		PieceAdapter.QueueFree();
		PiecesManager.Pieces.Remove(Query<IPositionable>().MapPosition, PieceAdapter);
	}

	protected override Operation _ToOperation(DisposeEvent @event)
	{
		return new()
		{
			PieceName = PieceAdapter.Name,
			Faction = PieceAdapter.Faction,
			Type = (int)OperationType.DISPOSE,
			CommentType = CommentType.GAME_DATA
		};
	}
}
