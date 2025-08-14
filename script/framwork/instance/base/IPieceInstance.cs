
using Godot;

/// <summary>
/// PieceInstance必须实现该接口
/// </summary>
public interface IPieceInstance : IPiece
{
  public Tween Tween { get; set; }
  public PieceAdapter PieceAdapter { get; set; }
  public TerrainLayers TerrainLayers { get; set; }
  public Vector2 AreaSize { get; set; }
  public Area2D Area { get; set; }
  void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false);
}
