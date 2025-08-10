using System;
using Godot;

public partial class PieceAdapter : Node, IPiece
{
  public event Action<int> ActionCompleted;

  public IPieceState State { get; protected set; }
  public IPieceInstance Instance { get; protected set; }

  public void Init(IPieceState state, IPieceInstance instance)
  {
    State = state;
    Instance = instance;
    if (Instance is PieceDecorator<IPieceInstance> decorator)
    {
      AddChild(decorator.Origin);
    }
    else
    {
      AddChild(Instance as Node);
    }
  }
}