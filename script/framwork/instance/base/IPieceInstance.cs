
using Godot;

/// <summary>
/// PieceInstance必须实现该接口
/// </summary>
public interface IPieceInstance : IPiece
{
  public Tween Tween { get; }

  void AddCover(Texture2D texture);
  
  void SetAreaSize(Vector2 size);
}
