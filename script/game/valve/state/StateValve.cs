public abstract partial class StateValve(IPieceState pieceState) : Valve
{
  protected IPieceState _pieceState = pieceState;
}