using System;
using System.Collections.Generic;
using Godot;

public partial class GameState : RefCounted
{
  public readonly static GameState Instance = new();

  public int PlayerFaction { get; set; }
  public string PlayerFactionName => Factions[PlayerFaction];
  public int CurOperatorFaction { get; set; }
  public string[] Factions { get; set; } = [];
  public int PlayerCount { get; set; }
  public bool IsSolo { get; set; }
  public RoomMetaData RoomMetaData { get; set; }
  public RoomState RoomState { get; set; }
  public PieceAdapter SelectedPiece { get; set; }
}