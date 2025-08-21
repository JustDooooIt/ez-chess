
using Godot;

/// <summary>
/// PieceInstance必须实现该接口
/// </summary>
public interface IPieceInstance : IPiece
{
  // public Tween Tween { get; set; }
  public PieceAdapter PieceAdapter { get; set; }
  public HexMap HexMap { get; set; }
  public Area2D Area { get; set; }
  public bool Selectable{ get; set; }
  public bool IsSelected { get; set; }
  public bool IsHover { get; set; }
  void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false);
}
