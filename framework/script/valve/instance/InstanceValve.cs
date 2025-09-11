public abstract partial class InstanceValve(IPieceInstance pieceInstance, Event @event) : Valve(pieceInstance)
{
  protected IPieceInstance _pieceInstance = pieceInstance;
  protected Event _event = @event;
}