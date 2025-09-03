using System.Collections.Generic;

public static class PieceStateExtensions
{
	public static IPieceState WithMovetState(this IPieceState piece, float movement)
			=> new MoveStateDecorator(piece, movement);
	public static IPieceState WithPositionState(this IPieceState piece)
		=> new PositionStateDecorator(piece);
	public static IPieceState WithAttackState(this IPieceState piece, float attackPoint)
		=> new AttackStateDecorator(piece, attackPoint);
	public static IPieceState WithRetreatState(this IPieceState piece)
		=> new RetreatStateDecorator(piece);
}