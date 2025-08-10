using System;
using System.Collections.Generic;
using Godot;

public partial class PieceAdapter : Node, IPiece
{
  public event Action<int> ActionCompleted;

  protected static Dictionary<ulong, ulong> _state_node_dict = [];
  protected static Dictionary<ulong, ulong> _node_state_dict = [];
  public IPieceState State { get; protected set; }
  public IPieceInstance Instance { get; protected set; }

  public void Init(IPieceState state, IPieceInstance instance)
  {
    State = state;
    Instance = instance;
    _state_node_dict[state.GetInstanceId()] = instance.GetInstanceId();
    _node_state_dict[instance.GetInstanceId()] = state.GetInstanceId();
    if (Instance is PieceInstanceDecorator decorator)
    {
      AddChild((Node)decorator.GetOrigin());
    } 
    else
    {
      AddChild(Instance as Node);
    }
  }

  public GodotObject GetOrigin()
  {
    return this;
  }

  public static ulong GetStateFromInstance(ulong id) { return _node_state_dict[id]; }

  public static ulong GetInstanceFromState(ulong id) { return _state_node_dict[id]; }
}