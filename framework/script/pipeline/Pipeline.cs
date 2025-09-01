using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

public abstract partial class Pipeline : Node, ILaunchable, IStopable
{
  [Signal]
  public delegate void ActionCompletedEventHandler();
  public event Action<Valve> ValveAdded;

  public abstract void Launch();
  public abstract void Stop();
  public abstract void AddValve(Valve launchable);
  protected void OnValveAdded(Valve valve)
  {
    ValveAdded?.Invoke(valve);
  }

  public enum States
  {
    IDLED, LAUNCHING, STOPED
  }
}