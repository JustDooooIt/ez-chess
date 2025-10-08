using Godot;

public interface IPositionable : IAction<PositionEvent>
{
  Vector2I MapPosition { get; set; }
}