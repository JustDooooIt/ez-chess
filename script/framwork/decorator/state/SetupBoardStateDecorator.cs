using Godot;

public class SetupBoardStateDecorator(IPieceState wrapped) : PieceStateDecorator(wrapped), ISetupBoard, IReadyPiece
{
  public Vector2I InitialPosition { get; set; }

  public void SetupBoard(Vector2I position)
  {
    ulong instanceId = PieceAdapter.GetInstanceFromState(Wrapped.GetInstanceId());
    var valve = new SetupBoardStateValve(this, new(instanceId, position));
    StatePipeline.AddValve(valve);
    RenderPipeline.RegisterValve<RenderSetupBoardEvent>(valve);
  }
}