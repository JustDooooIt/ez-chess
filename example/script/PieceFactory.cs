using System.Linq;
using Godot;
using Godot.Collections;

public partial  class PieceFactory : PieceFactoryBase, IPieceFactory
{
  protected override string PieceAdapterScenePath => IPieceFactory.PIECE_ADAPTER_PATH;

  protected override string PieceInstanceScenePath => IPieceFactory.PIECE_INSTANCE_PATH;

  public override PieceAdapter Create(int pieceType, params Variant[] args)
	{
		return pieceType switch
		{
			0 => CreateInfantry(args[0].AsGodotArray<Dictionary<string, Variant>>()),
			_ => null,
		};
	}

	private PieceAdapter CreateInfantry(Array<Dictionary<string, Variant>> data)
	{
		var piece = GD.Load<PackedScene>(PieceAdapterScenePath).Instantiate<PieceAdapter>();
		var instance = GD.Load<PackedScene>(PieceInstanceScenePath).Instantiate<PieceInstance>();
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
