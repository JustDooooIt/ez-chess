public static class PieceInstanceExtensions
{
    public static IPieceInstance WithMovetAction(this IPieceInstance piece)
        => new MoveInstanceDecorator(piece);
        
    public static IPieceInstance WithAttackAction(this IPieceInstance piece)
        => new AttackInstanceDecorator(piece);
}