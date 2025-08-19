using Godot;

public abstract partial class PieceInstanceDecorator(IPieceInstance wrapped) : PieceDecorator(wrapped), IPieceInstance
{
  private IPieceInstance _wrapped = wrapped;
  public PieceAdapter PieceAdapter { get => _wrapped.PieceAdapter; set => _wrapped.PieceAdapter = value; }
  public Area2D Area { get => _wrapped.Area; set => _wrapped.Area = value; }
  public bool Selectable { get => _wrapped.Selectable; set => _wrapped.Selectable = value; }
  public HexMap HexMap { get => _wrapped.HexMap; set => _wrapped.HexMap = value; }
  public bool IsSelected { get => _wrapped.IsSelected; set => _wrapped.IsSelected = value; }
  public bool IsHover { get => _wrapped.IsHover; set => _wrapped.IsHover = value; }


  public void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false)
  {
    _wrapped.AddCover(texture, faceIndex, defaultFace);
  }
}
