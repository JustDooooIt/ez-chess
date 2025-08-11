using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;

public partial class PlayerPipeline : PipelineImpl<ILaunchable>
{
  public override void RegisterValve<T>(Valve valve)
  {
    throw new System.NotImplementedException();
  }
}