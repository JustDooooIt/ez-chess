using System.Threading.Tasks;
using Godot;

public partial class RenderPipeline : PipelineImpl<InstanceValve>
{ 
  public void RegisterValve(Valve valve)
  {
    PipelineEventBus.Instance.Subscribe<MoveEvent>(valve.GetInstanceId(), HandleMoveEvent);
  }

  public void HandleMoveEvent(MoveEvent @event)
  {
    var piece = (PieceInstance)InstanceFromId(@event.pieceId);
    var valve = new MoveInstanceValve(piece);
    LaunchableList.Writer.WriteAsync(valve).AsTask().Wait();
  }
} 