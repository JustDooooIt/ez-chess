using System.Collections.Generic;

public static class PieceStateExtensions
{
    public static IPieceState WithMovetState(this IPieceState piece, List<float> movements)
        => new MoveStateDecorator(piece, movements);
	public static IPieceState WithPositionState(this IPieceState piece)
		=> new PositionStateDecorator(piece);
}