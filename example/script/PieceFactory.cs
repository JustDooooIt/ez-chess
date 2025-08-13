using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class PieceFactory : PieceFactoryBase, IPieceFactory
{
	public override PieceAdapter Create(int pieceType, string name, Array<Texture2D> images, int defaultFace, Vector2 areaSize, Array<Dictionary<string, Variant>> property)
	{
		return pieceType switch
		{
			0 => CreateInfantry(name, images, defaultFace, areaSize, property),
			_ => null,
		};
	}

	private PieceAdapter CreateInfantry(string name, Array<Texture2D> images, int defaultFace, Vector2 areaSize, Array<Dictionary<string, Variant>> property)
	{
		var piece = new PieceAdapter();
		var instance = GD.Load<PackedScene>(IPieceFactory.PIECE_INSTANCE_PATH).Instantiate<PieceInstance>();
		var stateWrapper = new PieceState()
			.WithSetupBoardState()
			.WithMovetState([.. property.Select(e => (float)e["move"].AsDouble())])
			.WithAttackState([.. property.Select(e => (float)e["attack"].AsDouble())]);
		var instanceWrapper = instance
			.WithSetupBoardAction()
			.WithMovetAction()
			.WithAttackAction();
		piece.Init(stateWrapper, instanceWrapper);
		PieceAddCover(piece, images, defaultFace);
		piece.Instance.SetAreaSize(areaSize);
		piece.Name = name;
		return piece;
	}
}
