using System;
using System.Collections.Generic;
using Godot;

public partial class PieceState : RefCounted, IPieceState
{
  public event Action<int> ActionCompleted;

  public GodotObject Origin => this;
  public IPiece Wrapped => this;
  public IInterfaceQueryable Wrapper { get; set; }
  public IInterfaceQueryable Proxy => ((IPiece)this).GetProxy();
  public PlayerPipeline PipelineAdapter { get; set; }
  public PiecesManager PiecesManager { get; set; }
  public PieceAdapter PieceAdapter { get; set; }
  public int Faction { get; set; }
  public int PieceType { get; set; }

  public IEnumerable<T> QueryAll<T>() where T : class
  {
	return [];
  }
}
