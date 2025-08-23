using Godot;


public record RenderEvent : Event
{
  public RenderEvent(ulong piece) : base(piece) { }
}

public record RenderMoveEvent : RenderEvent
{
  public Vector2I from;
  public Vector2I to;
  public Vector2I[] path;

  public RenderMoveEvent(ulong piece, Vector2I from, Vector2I to, Vector2I[] path) : base(piece)
  {
	this.from = from;
	this.to = to;
	this.path = path;
  }
}

public record RenderSetupBoardEvent : RenderEvent
{
  public Vector2I position;

  public RenderSetupBoardEvent(ulong piece, Vector2I position) : base(piece)
  {
	this.position = position;
  }
}
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
