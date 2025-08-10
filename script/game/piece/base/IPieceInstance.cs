
using Godot;

/// <summary>
/// PieceInstance必须实现该接口
/// </summary>
public interface IPieceInstance : IPiece
{
  public Tween Tween { get; }
}