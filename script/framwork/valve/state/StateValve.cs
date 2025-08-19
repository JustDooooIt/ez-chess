public abstract partial class StateValve : Valve
{
  protected IPieceState _pieceState;

  public StateValve(IPieceState pieceState) : base(pieceState)
  {
    _pieceState = pieceState;
    // StateChanged += OnStateChanged;
  }

  // private void OnStateChanged(ValveStates state)
  // {
  //   if (state == ValveStates.LAUNCHING)
  //   {
  //     GameState.Instance.RunningPiece = _pieceState;
  //   }
  //   else if (state == ValveStates.STOPED)
  //   {
  //     GameState.Instance.RunningPiece = null;
  //   }
  // }
}