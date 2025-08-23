using System;
using Godot;
using Godot.Collections;

public abstract partial class PieceFactoryBase : RefCounted, IPieceFactory
{
	public abstract PieceAdapter Create(int pieceType, int group, int faction, string name, Array<Texture2D> images, int defaultFace, Vector2 areaSize, Array<Dictionary<string, Variant>> property);

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
		int pieceType,
		int group,
		int faction,
		string name,
		Array<Texture2D> images,
		int defaultFace,
		Vector2 areaSize,
		Array<Dictionary<string, Variant>> property,
		Func<Array<Dictionary<string, Variant>>, (IPieceState state, IPieceInstance instance)> createAction) where T : PieceAdapter, new()
	{
		var piece = new T { Name = name };
		(var stateWrapper, var instanceWrapper) = createAction.Invoke(property);
		stateWrapper.Faction = faction;
		stateWrapper.PieceType = pieceType;
		instanceWrapper.Faction = faction;
		instanceWrapper.PieceType = pieceType;
		piece.Faction = faction;
		piece.PieceType = pieceType;
		piece.Group = group;
		piece.Init(stateWrapper, instanceWrapper);
		SetAreaSize(piece, areaSize);
		PieceAddCover(piece, images, defaultFace);
		return piece;
	}

	private void SetAreaSize(PieceAdapter piece, Vector2 areaSize)
	{
		var area = ((Node)piece.Instance.Origin).GetNode<Area2D>("Area2D");
		var shape = area.GetNode<CollisionShape2D>("CollisionShape2D");
		if (shape.Shape is RectangleShape2D rectangle)
		{
			rectangle.Size = areaSize;
		}
	}
}
