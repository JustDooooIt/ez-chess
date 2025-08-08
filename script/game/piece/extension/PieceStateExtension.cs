public static class PieceExtensions
{
    public static IPiece WithMovetState(this PieceState piece, float movement)
        => new MoveStateDecorator(piece, movement);
        
    public static IPiece WithAttackState(this PieceState piece, float attackPower)
        => new AttackStateDecorator(piece, attackPower);
}