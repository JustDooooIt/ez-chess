public abstract partial class StateValve(IPieceState pieceState) : Valve(pieceState)
{
  protected IPieceState _pieceState = pieceState;
}