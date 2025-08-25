using Godot;
using System;

public partial class Global : Node
{
  public static Global Instance { get; set; }

  public int Faction { get; set; }

  public Global()
  {
	Instance = this;
  }
}
