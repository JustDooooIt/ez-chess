using System;

public interface IActionEvent
{
  public event Action<int> ActionCompleted;
}