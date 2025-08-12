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
			case RenderSetupBoardEvent renderSetupBoardEvent:
				HandleSetupBoardEvent(renderSetupBoardEvent);
				break;
			default:
				break;
		}
	}

	public void HandleMoveEvent(RenderMoveEvent @event)
	{
		var valve = new MoveInstanceValve((PieceInstance)InstanceFromId(@event.pieceId), @event);
		LaunchableList.Writer.WriteAsync(valve).AsTask().Wait();
	}

	public void HandleSetupBoardEvent(RenderSetupBoardEvent @event)
	{
		var valve = new SetupBoardInstanceValve((PieceInstance)InstanceFromId(@event.pieceId), @event);
		LaunchableList.Writer.WriteAsync(valve).AsTask().Wait();
	}
}
