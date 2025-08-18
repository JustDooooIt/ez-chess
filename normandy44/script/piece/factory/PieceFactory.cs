using System.Linq;
using Godot;
using Godot.Collections;

public partial class PieceFactory : PieceFactoryBase, IPieceFactory
{
  public override PieceAdapter Create(int pieceType, string name, Array<Texture2D> images, int defaultFace, Vector2 areaSize, Array<Dictionary<string, Variant>> property)
  {
	return pieceType switch
	{
	  0 => Create<Infantry>(name, images, defaultFace, areaSize, property, CreateInfantry),
	  _ => default,
	};
  }

  private (IPieceState, IPieceInstance) CreateInfantry(Array<Dictionary<string, Variant>> property)
  {
	var instanceScene = GD.Load<PackedScene>(IPieceFactory.PIECE_INSTANCE_PATH);
	var state = new PieceState()
	  .WithPositionState()
	  .WithMovetState([.. property.Select(e=>(float)e["move"].AsDouble())]);
	var instance = instanceScene.Instantiate<PieceInstance>()
	  .WithPositionAction()
	  .WithMovetAction();
	return (state, instance);
  }
}
