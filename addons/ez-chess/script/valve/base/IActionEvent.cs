using System;

public interface IActionEvent
{
  public event Action<Valve.ValveStates> StateChanged;
}