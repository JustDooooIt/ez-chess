using System.Collections.Generic;

public class ValveGroup : ILaunchable
{
  private List<IValve> _valves;
  
  public void Launch()
  {
    foreach (var valve in _valves)
    {
      valve.Launch();
    }
  }
}