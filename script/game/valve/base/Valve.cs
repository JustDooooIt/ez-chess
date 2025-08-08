public abstract class Valve : IValve
{
  protected ValveStates _valveState;
  protected PieceState _pieceState;

  public abstract void Launch();

  public enum ValveStates
  { 
    IDLE, LAUNCH, STOP
  }
}