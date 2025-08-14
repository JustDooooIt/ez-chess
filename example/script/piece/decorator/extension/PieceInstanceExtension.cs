public static class PieceInstanceExtensions
{
	public static IPieceInstance WithMovetAction(this IPieceInstance piece)
		=> new MoveInstanceDecorator(piece);
		
	public static IPieceInstance WithSetupBoardAction(this IPieceInstance piece)
		=> new SetupBoardInstanceDecorator(piece);
}
