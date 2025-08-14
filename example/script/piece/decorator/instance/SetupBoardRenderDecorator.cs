using Godot;

public class SetupBoardInstanceDecorator(IPieceInstance wrapped) : PieceInstanceDecorator(wrapped), ISetupBoard
{
  public void SetupBoard(Vector2I position)
  {
    
  }
}