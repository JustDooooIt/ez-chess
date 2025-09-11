public static class PieceInstanceExtensions
{
	public static IPieceInstance WithMovetAction(this IPieceInstance piece)
		=> new MoveInstanceDecorator(piece);
	public static IPieceInstance WithPositionAction(this IPieceInstance piece)
		=> new PositionInstanceDecorator(piece);
	public static IPieceInstance WithAttackAction(this IPieceInstance piece)
		=> new AttackInstanceDecorator(piece);
	public static IPieceInstance WithRetreatAction(this IPieceInstance piece)
		=> new RetreatInstanceDecorator(piece);
	public static IPieceInstance WithAdvanceAction(this IPieceInstance piece)
		=> new AdvanceInstanceDecorator(piece);
}
