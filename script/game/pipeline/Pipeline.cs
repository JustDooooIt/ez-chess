using System.Threading;
using System.Threading.Tasks;
using Godot;

public abstract partial class Pipeline : Node, ILaunchable, IStopable
{

  public abstract Task Launch();
  public abstract Task Stop();
  public abstract void AddValve(IValve launchable);

  public enum States
  {
    IDLED, LAUNCHING, STOPED
  }
}