using Godot;

public interface IPlaceEventSender
{
  void SendPositionEvent(Vector2I position);
}