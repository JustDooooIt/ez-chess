public partial class DisposeStateValve(IPieceState pieceState, DisposeEvent @event) : StateValve(pieceState, @event)
{
  protected override void DoLaunch()
  {
	_pieceState.Query<IDisposable>().ReciveEvent(@event);
  }
}
