using Godot;

public abstract partial class PieceInstanceDecorator(IPieceInstance wrapped) : PieceDecorator(wrapped), IPieceInstance
{
  private IPieceInstance _wrapped = wrapped;
  public PieceAdapter PieceAdapter { get => _wrapped.PieceAdapter; set => _wrapped.PieceAdapter = value; }
  public TerrainLayers TerrainLayers { get => _wrapped.TerrainLayers; set => _wrapped.TerrainLayers = value; }
  public Vector2 AreaSize { get => _wrapped.AreaSize; set => _wrapped.AreaSize = value; }
  public Area2D Area { get => _wrapped.Area; set => _wrapped.Area = value; }
  public bool Selectable { get => _wrapped.Selectable; set => _wrapped.Selectable = value; }
  public HexMap HexMap { get => _wrapped.HexMap; set => _wrapped.HexMap = value; }

  public void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false)
  {
    _wrapped.AddCover(texture, faceIndex, defaultFace);
  }

  public void SetAreaSize(Vector2 size)
  {
    _wrapped.AreaSize = size;
  }

  public Vector2 GetAreaSize()
  {
    return _wrapped.AreaSize;
  }
}
