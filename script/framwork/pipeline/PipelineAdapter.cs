using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// 相当于player
/// </summary>
public partial class PipelineAdapter : Node
{
  public bool Operable { get; set; } = false;
  public Pipeline StatePipeline { get; private set; }
  public Pipeline RenderPipeline { get; private set; }

  public override void _Ready()
  {
    StatePipeline = GetChild<Pipeline>(0);
    RenderPipeline = GetChild<Pipeline>(1);
  }
}
