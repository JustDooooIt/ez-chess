using Godot;

public interface IAdvanceEventSender
{
  void SendAdvanceEvent(Vector2I from, Vector2I to);
}