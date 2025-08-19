using System;
using System.Collections.Generic;
using Godot;

public interface IPiece : IInterfaceQueryable
{
  PipelineAdapter PipelineAdapter { get; set; }
  PiecesManager PiecesManager { get; set; }
  int Faction { get; set; }
  int PieceType{ get; set; }
  ulong GetInstanceId();
}
