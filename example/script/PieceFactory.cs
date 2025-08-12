using System.Linq;
using Godot;
using Godot.Collections;

public partial class PieceFactory : RefCounted, IPieceFactory
{
	public PieceAdapter Create(int pieceType, params Variant[] args)
	{
		return pieceType switch
		{
			0 => CreateInfantry(args[0].AsGodotArray<Dictionary<string, Variant>>()),
			_ => null,
		};
	}

	private static PieceAdapter CreateInfantry(Array<Dictionary<string, Variant>> data)
	{
		var piece = GD.Load<PackedScene>(IPieceFactory.PIECE_ADAPTER_PATH).Instantiate<PieceAdapter>();
		var instance = GD.Load<PackedScene>(IPieceFactory.PIECE_INSTANCE_PATH).Instantiate<PieceInstance>();
		var stateWrapper = new PieceState()
			.WithMovetState([.. data.Select(e => (float)e["move"].AsDouble())])
			.WithAttackState([.. data.Select(e => (float)e["attack"].AsDouble())]);
		var instanceWrapper = instance
			.WithMovetAction()
			.WithAttackAction();
		piece.Init(stateWrapper, instanceWrapper);
		return piece;
	}
}
