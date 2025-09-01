using Godot;


public record PieceEvent : Event
{
  //标记该事件是否是恢复阶段的事件
  public bool recovered;
  public PieceEvent(ulong piece, bool recovered = false) : base(piece)
  {
    this.recovered = recovered;
  }
}

public record MoveEvent : PieceEvent
{
  public Vector2I from;
  public Vector2I to;
  public Vector2I[] path;

  public MoveEvent(ulong piece, Vector2I from, Vector2I to, Vector2I[] path, bool recovered = false) : base(piece, recovered)
  {
    this.from = from;
    this.to = to;
    this.path = path;
  }
}

public record SetupBoardEvent : PieceEvent
{
  public Vector2I position;

  public SetupBoardEvent(ulong piece, Vector2I position) : base(piece)
  {
    this.position = position;
  }
}
public record FlipEvent : PieceEvent
{
  public FlipEvent(ulong piece) : base(piece)
  {

  }
}

public record PositionEvent : PieceEvent
{
  public Vector2I position;
  public PositionEvent(ulong piece, Vector2I position) : base(piece)
  {
    this.position = position;
  }
}
