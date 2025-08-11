using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;

public partial class PipelineAdapter : Node
{
  private Pipeline _state;
  private Pipeline _render;

  public override void _Ready()
  {
	_state = GetChild<Pipeline>(0);
	_render = GetChild<Pipeline>(1);
  }
}
