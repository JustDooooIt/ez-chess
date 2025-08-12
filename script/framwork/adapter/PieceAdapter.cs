using System;
using System.Collections.Generic;
using Godot;

public partial class PieceAdapter : Node, IPiece
{
  public event Action<int> ActionCompleted;

  private readonly static Dictionary<ulong, ulong> _state_node_dict = [];
  private readonly static Dictionary<ulong, ulong> _node_state_dict = [];
  private Pipeline _statePipeline;
  private Pipeline _renderPipeline;

  public IPieceState State { get; protected set; }
  public IPieceInstance Instance { get; protected set; }
  public Pipeline StatePipeline { get => _statePipeline; set => SetStatePipeline(value); }
  public Pipeline RenderPipeline { get => _renderPipeline; set => SetRenderPipeline(value); }
  GodotObject IPiece.Origin => this;


  public void Init(IPieceState state, IPieceInstance instance)
  {
	State = state;
	Instance = instance;
	_state_node_dict[state.GetInstanceId()] = instance.GetInstanceId();
	_node_state_dict[instance.GetInstanceId()] = state.GetInstanceId();
	if (Instance is PieceInstanceDecorator decorator)
	{
	  AddChild((Node)decorator.Origin);
	}
	else
	{
	  AddChild(Instance as Node);
	}
  }

  public static ulong GetStateFromInstance(ulong id) { return _node_state_dict[id]; }

  public static ulong GetInstanceFromState(ulong id) { return _state_node_dict[id]; }

  private void SetStatePipeline(Pipeline pipeline)
  {
	State.StatePipeline = pipeline;
	Instance.StatePipeline = pipeline;
	_statePipeline = pipeline;
  }

  private void SetRenderPipeline(Pipeline pipeline)
  {
	State.RenderPipeline = pipeline;
	Instance.RenderPipeline = pipeline;
	_renderPipeline = pipeline;
  }
}
