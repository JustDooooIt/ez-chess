public abstract partial class InstanceValve(IPieceInstance pieceInstance) : Valve(pieceInstance)
{
  protected IPieceInstance _pieceInstance = pieceInstance;
}