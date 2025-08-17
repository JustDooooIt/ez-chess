using Godot;

public partial class Infantry : PieceAdapter
{
  public override void _Ready()
  {
    base._Ready();

  }

  public override void _UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseButton mouseButton)
    {
      
    }
  }
}