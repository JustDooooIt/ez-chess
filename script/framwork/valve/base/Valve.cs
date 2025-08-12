using System.Threading.Tasks;
using Godot;

public abstract partial class Valve : RefCounted, IValve
{
  public ValveStates ValveState { get; protected set; } = ValveStates.IDLED;

  public async Task Launch()
  {
    ValveState = ValveStates.LAUNCHING;
    DoLaunch();
    ValveState = ValveStates.STOPED;
  }

  protected abstract void DoLaunch();

  public enum ValveStates
  {
    IDLED, LAUNCHING, STOPED
  }
}