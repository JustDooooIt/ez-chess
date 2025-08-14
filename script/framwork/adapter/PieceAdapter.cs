using System;
using System.Collections.Generic;
using Godot;

public partial class PieceAdapter : Node
{
  public event Action<int> ActionCompleted;

  private readonly static Dictionary<ulong, ulong> _state_node_dict = [];
  private readonly static Dictionary<ulong, ulong> _node_state_dict = [];
  private PipelineAdapter _pipelineAdapter;
  private PiecesManager _piecesManager;
  private GodotObject _origin;

  public IPieceState State { get; protected set; }
  public IPieceInstance Instance { get; protected set; }
  public PipelineAdapter PipelineAdapter { get; set; }
  public PiecesManager PiecesManager { get => _piecesManager; set => _piecesManager = value; }
  public int Faction { get; set; }

  public override void _Ready()
  {
	Faction = GetParent().GetIndex();
	SetPiecesManager();
	SetPipelineAdapter();
	SetPieceAdapter();
  }

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

  private void SetPipelineAdapter()
  {
	PipelineAdapter = GetNode<PipelineAdapter>($"../../../Players/{_piecesManager.Name}");
	State.PipelineAdapter = PipelineAdapter;
	Instance.PipelineAdapter = PipelineAdapter;
	State.PiecesManager = PiecesManager;
	Instance.PiecesManager = PiecesManager;
  }
  private void SetPiecesManager()
  {
	PiecesManager = GetNode<PiecesManager>("..");
	State.PiecesManager = PiecesManager;
	Instance.PiecesManager = PiecesManager;
  }

  private void SetPieceAdapter()
  {
	State.PieceAdapter = this;
	Instance.PieceAdapter = this;
  }
}
