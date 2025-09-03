using Godot;

public interface IAttackEventSender
{
  void SendAttackEvent(Vector2I target, PieceAdapter targetPiece);
}