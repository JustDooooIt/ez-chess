using Godot;

public interface IMoveEventSender
{
  void SendMoveEvent(Vector2I from, Vector2I to);
}