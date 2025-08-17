using Godot;

public record RenderFlipEvent : RenderEvent
{
  public RenderFlipEvent(ulong piece) : base(piece)
  {

  }
}

public record RenderPositionEvent : RenderEvent
{
  public Vector2I position;
  public RenderPositionEvent(ulong piece, Vector2I position) : base(piece)
  {
    this.position = position;
  }
}