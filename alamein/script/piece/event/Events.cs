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

public record RetreatEvent : PieceEvent
{
  public Vector2I from;
  public Vector2I to;
  public Vector2I[] path;

  public RetreatEvent(ulong piece, Vector2I from, Vector2I to, Vector2I[] path, bool recovered = false) : base(piece, recovered)
  {
    this.from = from;
    this.to = to;
    this.path = path;
  }
}

public record AttackEvent : PieceEvent
{
  public ulong targetPiece;
  public Vector2I from;
  public Vector2I target;
  public AttackEvent(Vector2I from, ulong pieceId, Vector2I target, ulong targetPiece) : base(pieceId, false)
  {
    this.targetPiece = targetPiece;
    this.from = from;
    this.target = target;
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
