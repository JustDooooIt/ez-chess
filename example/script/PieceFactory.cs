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
			0 => CreateGeneralPiece(name, images, defaultFace, areaSize, property),
			_ => null,
		};
	}

	private PieceAdapter CreateGeneralPiece(string name, Array<Texture2D> images, int defaultFace, Vector2 areaSize, Array<Dictionary<string, Variant>> property)
	{
		return Create<GeneralPiece>(name, images, defaultFace, areaSize, property, CreateGeneralPiece);
	}

	private (IPieceState, IPieceInstance) CreateGeneralPiece(Array<Dictionary<string, Variant>> property)
	{
		var instance = GD.Load<PackedScene>(IPieceFactory.PIECE_INSTANCE_PATH).Instantiate<PieceInstance>();
		var stateWrapper = new PieceState()
			.WithSetupBoardState()
			.WithMovetState([.. property.Select(e => (float)e["move"].AsDouble())]);
		var instanceWrapper = instance
			.WithSetupBoardAction()
			.WithMovetAction();
		return (stateWrapper, instanceWrapper);
	}
}
