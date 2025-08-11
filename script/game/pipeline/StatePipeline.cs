using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;

public partial class StatePipeline : PipelineImpl<StateValve>
{
  public override void RegisterValve<T>(Valve valve)
  {
	throw new System.NotImplementedException();
  }
}
