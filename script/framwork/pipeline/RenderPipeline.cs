using System.Threading.Tasks;
using Godot;

public partial class RenderPipeline : PipelineImpl<InstanceValve>
{
  public override void RegisterValve<T>(Valve valve)
  { 
	PipelineEventBus.Instance.Subscribe<T>(valve.GetInstanceId(), HandleEvent);
  }

  public void HandleEvent<T>(T @event)
  {
	switch (@event)
	{
	  case RenderMoveEvent renderMoveEvent:
		HandleMoveEvent(renderMoveEvent);
		break;
	  default:
		break;
	}
  }

  public void HandleMoveEvent(RenderMoveEvent @event)
  {
	var valve = new MoveInstanceValve(InstanceFromId(@event.pieceId) as PieceInstance, @event);
	LaunchableList.Writer.WriteAsync(valve).AsTask().Wait();
  }
}
