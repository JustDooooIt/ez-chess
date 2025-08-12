using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class PieceFactory : PieceFactoryBase, IPieceFactory
{
	public override PieceAdapter Create(int pieceType, params Variant[] args)
	{
		return pieceType switch
		{
			0 => CreateInfantry(args[0].AsString(), args[1].AsGodotArray<Texture2D>(), args[2].AsInt16(), args[3].AsVector2(), args[4].AsGodotArray<Dictionary<string, Variant>>()),
			_ => null,
		};
	}

	private PieceAdapter CreateInfantry(string name, Array<Texture2D> images, int defaultFace, Vector2 areaSize, Array<Dictionary<string, Variant>> data)
	{
		var piece = new PieceAdapter();
		var instance = GD.Load<PackedScene>(IPieceFactory.PIECE_INSTANCE_PATH).Instantiate<PieceInstance>();
		var stateWrapper = new PieceState()
			.WithSetupBoardState()
			.WithMovetState([.. data.Select(e => (float)e["move"].AsDouble())])
			.WithAttackState([.. data.Select(e => (float)e["attack"].AsDouble())]);
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
