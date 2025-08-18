public static class PieceInstanceExtensions
{
	public static IPieceInstance WithMovetAction(this IPieceInstance piece)
		=> new MoveInstanceDecorator(piece);
	public static IPieceInstance WithPositionAction(this IPieceInstance piece)
		=> new PositionInstanceDecorator(piece);
}
