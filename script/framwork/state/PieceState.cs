using System;
using Godot;

public partial class PieceState : RefCounted, IPieceState
{
  public event Action<int> ActionCompleted;

  public GodotObject Origin => this;
  public IInterfaceQueryable Proxy => ((IPiece)this).GetProxy();
  public PipelineAdapter PipelineAdapter { get; set; }
  public PiecesManager PiecesManager { get; set; }
  public PieceAdapter PieceAdapter { get; set; }
  public IInterfaceQueryable Wrapper { get; set; }

  public void SetupBoard(Vector2I position)
  {
    // As<IReadyPiece>().InitialPosition = position;
    // PiecesManager.AddPiece(position, _pieceState.PieceAdapter);
  }
}
