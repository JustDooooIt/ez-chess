using Godot;

public abstract class PieceInstanceDecorator(IPieceInstance wrapped) : PieceDecorator<IPieceInstance>(wrapped), IPieceInstance
{
  public Tween Tween => (_wrapped as IPieceInstance).Tween;

  public bool Selectable { get => Wrapped.Selectable; set => Wrapped.Selectable = value; }
  public PieceAdapter PieceAdapter { get => Wrapped.PieceAdapter; set => Wrapped.PieceAdapter = value; }
  public TerrainLayers TerrainLayers { get => Wrapped.TerrainLayers; set => Wrapped.TerrainLayers = value; }
  public Vector2 AreaSize { get => Wrapped.AreaSize; set => Wrapped.AreaSize = value; }


  public void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false)
  {
    Wrapped.AddCover(texture, faceIndex, defaultFace);
  }

  public void SetAreaSize(Vector2 size)
  {
    Wrapped.AreaSize = size;
  }

  public Vector2 GetAreaSize()
  {
    return Wrapped.AreaSize;
  }
}
