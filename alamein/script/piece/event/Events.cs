using Godot;


public record PieceEvent : Event
{
  //标记该事件是否是恢复阶段的事件
  public bool recovered;

  public PieceEvent(bool recovered = false)
  {
    this.recovered = recovered;
  }
}

public record MoveEvent : PieceEvent
{
  public Vector2I from;
  public Vector2I to;
  public Vector2I[] path;
  public int faction;
  public string pieceName;
  public float cost;

  public MoveEvent(int faction, string pieceName, Vector2I from, Vector2I to, float cost, Vector2I[] path, bool recovered = false) : base(recovered)
  {
    this.from = from;
    this.to = to;
    this.path = path;
    this.faction = faction;
    this.pieceName = pieceName;
    this.cost = cost;
  }
}

public record RetreatEvent : PieceEvent
{
  public Vector2I from;
  public Vector2I to;
  public Vector2I[] path;
  public int faction;
  public string pieceName;

  public RetreatEvent(int faction, string pieceName, Vector2I from, Vector2I to, Vector2I[] path, bool recovered = false) : base(recovered)
  {
    this.from = from;
    this.to = to;
    this.path = path;
    this.faction = faction;
    this.pieceName = pieceName;
  }
}

public record AttackEvent : PieceEvent
{
  public string fromPiece;
  public string targetPiece;
  public Vector2I from;
  public Vector2I target;
  public int fromFaction;
  public int targetFaction;

  public AttackEvent(Vector2I from, int fromFaction, string fromPiece, Vector2I target, int targetFaction, string targetPiece) : base(false)
  {
    this.targetPiece = targetPiece;
    this.from = from;
    this.target = target;
    this.fromFaction = fromFaction;
    this.targetFaction = targetFaction;
    this.fromPiece = fromPiece;
  }
}

public record DisposeEvent : PieceEvent
{
  public int faction;
  public string piece;
  public DisposeEvent(int faction, string piece, bool recovered = false) : base(recovered)
  {
    this.faction = faction;
    this.piece = piece;
  }
}

public record PositionEvent : PieceEvent
{
  public Vector2I position;
  public int faction;
  public string pieceName;

  public PositionEvent(int faction, string pieceName, Vector2I position) : base()
  {
    this.position = position;
    this.faction = faction;
    this.pieceName = pieceName;
  }
}

public record ResetEvent : PieceEvent
{
  public ResetEvent(bool recovered) : base(recovered) { }
}