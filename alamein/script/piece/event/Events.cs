using Godot;


public record PieceEvent : Event
{
  //标记该事件是否是恢复阶段的事件
  public bool recovered;
  public int faction;
  public string pieceName;

  public PieceEvent(int faction, string pieceName, bool recovered = false)
  {
    this.recovered = recovered;
    this.faction = faction;
    this.pieceName = pieceName;
  }
}

public record MoveEvent : PieceEvent
{
  public Vector2I from;
  public Vector2I to;
  public Vector2I[] path;
  public float cost;

  public MoveEvent(int faction, string pieceName, Vector2I from, Vector2I to, float cost, Vector2I[] path, bool recovered = false) : base(faction, pieceName, recovered)
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

  public RetreatEvent(int faction, string pieceName, Vector2I from, Vector2I to, Vector2I[] path, bool recovered = false) : base(faction, pieceName, recovered)
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
  // public int combatResult;

  public AttackEvent(Vector2I from, int fromFaction, string fromPiece, Vector2I target, int targetFaction, string targetPiece) : base(fromFaction, fromPiece, false)
  {
    this.targetPiece = targetPiece;
    this.from = from;
    this.target = target;
    this.fromFaction = fromFaction;
    this.targetFaction = targetFaction;
    this.fromPiece = fromPiece;
  }
  // public AttackEvent(Vector2I from, int fromFaction, string fromPiece, Vector2I target, int targetFaction, string targetPiece, int CombatResult) : base(fromFaction, fromPiece, false)
  // {
  //   this.targetPiece = targetPiece;
  //   this.from = from;
  //   this.target = target;
  //   this.fromFaction = fromFaction;
  //   this.targetFaction = targetFaction;
  //   this.fromPiece = fromPiece;
  //   this.combatResult = CombatResult;
  // }
}

public record DisposeEvent : PieceEvent
{
  public string piece;
  public DisposeEvent(int faction, string piece, bool recovered = false) : base(faction, piece, recovered)
  {
    this.faction = faction;
    this.piece = piece;
  }
}

public record PositionEvent : PieceEvent
{
  public Vector2I position;

  public PositionEvent(int faction, string pieceName, Vector2I position, bool recovered = false) : base(faction, pieceName, recovered)
  {
    this.position = position;
    this.faction = faction;
    this.pieceName = pieceName;
  }
}

public record ResetEvent : PieceEvent
{
  public ResetEvent(int faction, string pieceName, bool recovered) : base(faction, pieceName, recovered) { }
}

public record AdvanceEvent : PieceEvent
{
  public Vector2I from;
  public Vector2I to;
  public AdvanceEvent(int faction, string pieceName, Vector2I from, Vector2I to, bool recovered) : base(faction, pieceName, recovered)
  {
    this.from = from;
    this.to = to;
  }
}