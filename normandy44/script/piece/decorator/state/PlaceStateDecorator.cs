using Godot;

public partial class PlaceStateDecorator(IPieceState wrapped) : PieceStateDecorator(wrapped), IPlaceable, IPlaceEventSender
{
  public void Place(Vector2I position)
  {
    Wrapped.As<IPositionable>().MapPosition = position;
  }

  public void SendPositionEvent(Vector2I position)
  {
    As<IPositionable>().MapPosition = position;
  }
}