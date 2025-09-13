using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class GameState : RefCounted
{
  public readonly static GameState Instance = new();
  public event Action<int> StageChanged;
  public event Action<int> TurnChanged;

  private int _stage = 0;
  private int _turn = 0;

  public Dictionary Config { get; set; } = [];
  public int Turn { get => _turn; set => SetTurn(value); }
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

  private void SetTurn(int turn)
  {
    this._turn = turn;
    TurnChanged?.Invoke(turn);
  }

  public void EndStage()
  {
    Stage = (Stage + 1) % StageCount;
  }

  public enum Stages
  {
    MOVE, ATTACK
  }
}