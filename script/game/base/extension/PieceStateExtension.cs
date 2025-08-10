public static class PieceStateExtensions
{
    public static IPieceState WithMovetState(this IPieceState piece, float movement)
        => new MoveStateDecorator(piece, movement);
        
    public static IPieceState WithAttackState(this IPieceState piece, float attackPower)
        => new AttackStateDecorator(piece, attackPower);
}