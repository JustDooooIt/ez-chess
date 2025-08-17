public partial class FlipableInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator(wrapped), IFlipable
{
  public void Flip()
  {
    throw new System.NotImplementedException();
  }

  public void SendFlipEvent()
  {
    throw new System.NotImplementedException();
  }
}