using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;

public partial class RenderPipeline : PipelineImpl<InstanceValve>
{
	public BaseRenderEventHandler RenderEventHandler { get; set; }

	public void RegisterValve<T>(Valve valve) where T : Event
	{
		PipelineEventBus.Instance.Subscribe<T>(valve.GetInstanceId(), RenderEventHandler.HandleEvent);
	}
}

public abstract partial class BaseRenderEventHandler : RefCounted
{
	public RenderPipeline Pipeline { get; set; }

	public void HandleEvent<T>(T @event) where T : Event
	{
		InstanceValve valve = CreateValve(@event);
		Pipeline.LaunchableList.Writer.WriteAsync(valve).AsTask().Wait();
	}

	protected abstract InstanceValve CreateValve<T>(T @event) where T : Event;
}
