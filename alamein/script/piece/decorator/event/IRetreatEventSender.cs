using Godot;

public interface IRetreatEventSender
{
  void SendRetreatEvent(Vector2I from, Vector2I to, bool recovered = false);
}