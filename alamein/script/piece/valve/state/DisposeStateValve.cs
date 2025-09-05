public partial class DisposeStateValve(IPieceState pieceState, DisposeEvent @event) : StateValve(pieceState)
{
  protected override void DoLaunch()
  {
    _pieceState.As<IDisposable>().ReciveEvent(@event);
  }
}