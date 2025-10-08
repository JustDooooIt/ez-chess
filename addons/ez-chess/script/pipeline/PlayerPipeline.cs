using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// 相当于player
/// </summary>
public partial class PlayerPipeline : Node
{
  public GameManager GameManager { get; set; }
  public StatePipeline StatePipeline { get; private set; }
  public RenderPipeline RenderPipeline { get; private set; }
  public override void _Ready()
  {
    StatePipeline = GetChild<StatePipeline>(0);
    RenderPipeline = GetChild<RenderPipeline>(1);
    GameManager = GetNode<GameManager>("/root/Game");
  }
}
