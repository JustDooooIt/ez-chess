using System.Collections.Generic;
using Godot;

public partial class GameState : RefCounted
{
  public readonly static GameState Instance = new();

  public int PlayerFaction { get; set; }
  public int CurOperator { get; set; }
  public string[] Factions { get; set; } = [];
}