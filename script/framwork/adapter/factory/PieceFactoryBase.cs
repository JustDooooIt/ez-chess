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

	protected PieceAdapter Create<T>(
		string name,
		Array<Texture2D> images,
		int defaultFace,
		Vector2 areaSize,
		Array<Dictionary<string, Variant>> property,
		Func<Array<Dictionary<string, Variant>>, (IPieceState state, IPieceInstance instance)> createAction) where T : PieceAdapter, new()
	{
		var piece = new T { Name = name };
		(var stateWrapper, var instanceWrapper) = createAction.Invoke(property);
		piece.Init(stateWrapper, instanceWrapper);
		piece.Instance.AreaSize = areaSize;
		PieceAddCover(piece, images, defaultFace);
		return piece;
	}
}
