using Godot;

public interface IPositionEventSender
{
  void SendPositionEvent(Vector2I position);
}