using Godot;
using Godot.Collections;

public abstract partial class PieceFactoryBase : RefCounted, IPieceFactory
{
  public abstract PieceAdapter Create(int pieceType, params Variant[] args);

  protected void PieceAddCover(PieceAdapter piece, Array<Texture2D> images, int defaultFace)
  {
    piece.Instance.AddCover(images[defaultFace], defaultFace, true);
    for (int i = 0; i < images.Count; i++)
    {
      if (i == defaultFace)
        continue;
      piece.Instance.AddCover(images[defaultFace], i);
    }
  }
}