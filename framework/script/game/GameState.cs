using System;
using System.Collections.Generic;
using Godot;

public partial class GameState : RefCounted
{
  public readonly static GameState Instance = new();
  public event Action<int> StageChanged;

  private int _stage = 0;

  GameState()
  {
    Stage = 0;
  }

  public int Turn { get; set; } = 0;
  public int StageCount { get; set; } = 0;
  public int Stage { get => _stage; set => SetStage(value); }
  public string Username { get; set; }
  public int PlayerFaction { get; set; }
  public string PlayerFactionName => Factions[PlayerFaction];
  public int CurOperatorFaction { get; set; } = 0;
  public string[] Factions { get; set; } = [];
  public int PlayerCount { get; set; }
  public bool IsSolo { get; set; } = true;
  public RoomMetaData RoomMetaData { get; set; }
  public RoomState RoomState { get; set; }
  public PieceAdapter SelectedPiece { get; set; }
  public PieceAdapter HoveredPiece { get; set; }

  private void SetStage(int stage)
  {
    _stage = stage;
    StageChanged?.Invoke(stage);
  }

  public void EndStage()
  {
    Stage = (Stage + 1) % StageCount;
    GD.Print(Stage);
  }

  public enum Stages
  {
    MOVE, ATTACK
  }
}