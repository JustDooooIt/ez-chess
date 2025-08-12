using Godot;

public abstract class PieceInstanceDecorator(IPieceInstance wrapped) : PieceDecorator<IPieceInstance>(wrapped), IPieceInstance
{
  public Tween Tween => (_wrapped as IPieceInstance).Tween;

  public bool Selectable { get => ((IPieceInstance)Origin).Selectable; set => ((IPieceInstance)Origin).Selectable = value; }

  public void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false)
  {
    Wrapped.AddCover(texture, faceIndex, defaultFace);
  }

  public void SetAreaSize(Vector2 size)
  {
    Wrapped.SetAreaSize(size);
  }

}
