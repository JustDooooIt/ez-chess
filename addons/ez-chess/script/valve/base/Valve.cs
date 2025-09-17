using System;
using System.Threading.Tasks;
using Godot;

public abstract partial class Valve(IPiece piece) : RefCounted, IValve, IActionEvent
{
  [Signal]
  public delegate void ValveCompletedEventHandler();

  public event Action<ValveStates> StateChanged;

  private IPiece _piece = piece;
  private ValveStates _valveState = ValveStates.IDLED;
  public ValveStates ValveState { get => _valveState; protected set => SetVlaveState(value); }
  public void Launch()
  {
	ValveState = ValveStates.LAUNCHING;
	DoLaunch();
	EmitSignal("ValveCompleted");
	ValveState = ValveStates.STOPED;
  }

  protected abstract void DoLaunch();

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
