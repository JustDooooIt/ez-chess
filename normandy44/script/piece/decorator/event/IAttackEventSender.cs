using Godot;

public interface IAttackEventSender
{
  void SendAttackEvent(Vector2I from, Vector2I to);
}