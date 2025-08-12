using Godot;

public abstract class PieceInstanceDecorator(IPieceInstance wrapped) : PieceDecorator<IPieceInstance>(wrapped), IPieceInstance
{
  public Tween Tween => (_wrapped as IPieceInstance).Tween;

  public void AddCover(Texture2D texture)
  {
    Wrapped.AddCover(texture);
  }

  public void SetAreaSize(Vector2 size)
  {
    Wrapped.SetAreaSize(size);
  }

}
