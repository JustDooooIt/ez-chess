using System;
using Godot;
using Godot.Collections;

public abstract partial class PieceFactoryBase : RefCounted, IPieceFactory
{
  public abstract PieceAdapter Create(int pieceType, string name, Array<Texture2D> images, int defaultFace, Vector2 areaSize, Array<Dictionary<string, Variant>> property);

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

  protected PieceAdapter CreateInternal(string name,
    Array<Texture2D> images,
    int defaultFace,
    Vector2 areaSize,
    Array<Dictionary<string, Variant>> property,
    Func<Array<Dictionary<string, Variant>>,(IPieceState state, IPieceInstance instance)> createAction)
  {
    var piece = new PieceAdapter { Name = name };
    (var stateWrapper, var instanceWrapper) = createAction.Invoke(property);
    piece.Init(stateWrapper, instanceWrapper);
    PieceAddCover(piece, images, defaultFace);
    piece.Instance.AreaSize = areaSize;
    return piece;
  }
}