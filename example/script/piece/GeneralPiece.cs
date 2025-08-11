using System.Threading.Tasks;
using Godot;

public partial class GeneralPiece : PieceAdapter
{
	private const string scenePath = "res://example/scene/general_piece.tscn";

	private GameManager gameManager;

	public override void _Ready()
	{
		var state = new PieceState()
			.WithAttackState(5)
			.WithMovetState(5);

		var instance = GD.Load<PackedScene>(scenePath)
			.Instantiate<GeneralInstance>()
			.WithMovetAction()
			.WithAttackAction();

		Init(state, instance);

		// Test
		gameManager = GetNode<GameManager>("../..");
		Move();
	}

	public void Move()
	{
		State.As<IMoveable>()?.Move(new(0, 0), new(100, 100));
	}
}
