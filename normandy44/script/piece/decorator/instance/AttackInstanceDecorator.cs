public partial class AttackInstanceDecorator : PieceInstanceDecorator
{
  private IPieceInstance _wrapped;
  
  public AttackInstanceDecorator(IPieceInstance wrapped) : base(wrapped)
  {
    _wrapped = wrapped;
  }
}