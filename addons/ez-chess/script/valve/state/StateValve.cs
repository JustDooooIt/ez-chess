public abstract partial class StateValve(IPieceState pieceState, PieceEvent @event) : Valve(pieceState)
{
  protected IPieceState _pieceState = pieceState;
  protected PieceEvent _event = @event;
}