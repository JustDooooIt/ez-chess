using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;

public partial class PipelineAdapter : Node
{
  public Pipeline StatePipeline { get; private set; }
  public Pipeline RenderPipeline { get; private set; }

  public override void _Ready()
  {
	StatePipeline = GetChild<Pipeline>(0);
	RenderPipeline = GetChild<Pipeline>(1);
  }
}
