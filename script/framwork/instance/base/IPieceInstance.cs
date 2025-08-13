
using Godot;

/// <summary>
/// PieceInstance必须实现该接口
/// </summary>
public interface IPieceInstance : IPiece
{
  public Tween Tween { get; }

  bool Selectable { get; set; }
  public PieceAdapter PieceAdapter{ get; set; }
  void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false);
  void SetAreaSize(Vector2 size);
}
