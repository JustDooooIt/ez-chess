using Godot;

public class PieceInstanceDecorator(IPieceInstance wrapped) : PieceDecorator<IPieceInstance>(wrapped), IPieceInstance
{
  public Tween Tween => (_wrapped as IPieceInstance).Tween;
}
