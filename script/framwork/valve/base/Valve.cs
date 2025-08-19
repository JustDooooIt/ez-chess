using System;
using System.Threading.Tasks;
using Godot;

public abstract partial class Valve(IPiece piece) : RefCounted, IValve, IActionEvent
{
  private IPiece _piece = piece;
  private ValveStates _valveState = ValveStates.IDLED;
  public ValveStates ValveState { get => _valveState; protected set=>SetVlaveState(value); }
  public event Action<ValveStates> StateChanged;

  public async Task Launch()
  {
    ValveState = ValveStates.LAUNCHING;
    await DoLaunch();
    ValveState = ValveStates.STOPED;
  }

  protected abstract Task DoLaunch();

  private void SetVlaveState(ValveStates state)
  {
    _valveState = state;
    StateChanged?.Invoke(state);
  }

  public enum ValveStates
  {
    IDLED, LAUNCHING, STOPED
  }
}