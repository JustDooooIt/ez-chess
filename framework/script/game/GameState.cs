using System.Collections.Generic;
using Godot;

public partial class GameState : RefCounted
{
  public readonly static GameState Instance = new();

  public string Username { get; set; }
  public int PlayerFaction { get; set; }
  public int CurOperatorFaction { get; set; }
  public string[] Factions { get; set; } = [];
  public int PlayerCount { get; set; } = 0;
  public PieceAdapter SelectedPiece { get; set; }
}