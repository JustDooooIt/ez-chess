using System;
using System.Collections.Generic;
using Godot;

public partial class PieceAdapter : Node
{

  private readonly static Dictionary<ulong, ulong> _state_node_dict = [];
  private readonly static Dictionary<ulong, ulong> _node_state_dict = [];
  private GodotObject _origin;

  public IPieceState State { get; protected set; }
  public IPieceInstance Instance { get; protected set; }
  public PlayerPipeline PipelineAdapter { get; set; }
  public GameManager GameManager { get; set; }
  public PiecesManager PiecesManager { get; set; }
  public HexMap HexMap { get; set; }
  public int Faction { get; set; }
  public int PieceType { get; set; }
  public int Group { get; set; }

  public override void _Ready()
  {
    SetPiecesManager();
    SetPipelineAdapter();
    SetPieceAdapter();
    SetHexMap();
  }

  public void Init(IPieceState state, IPieceInstance instance)
  {
    State = state;
    Instance = instance;
    _state_node_dict[state.Origin.GetInstanceId()] = instance.Origin.GetInstanceId();
    _node_state_dict[instance.Origin.GetInstanceId()] = state.Origin.GetInstanceId();
    if (Instance is Node node)
    {
      AddChild(node);
    }
    else
    {
      AddChild(instance.Origin as Node);
    }
  }

  public static ulong GetStateFromInstance(ulong id) { return _node_state_dict[id]; }

  public static ulong GetInstanceFromState(ulong id) { return _state_node_dict[id]; }

  private void SetPipelineAdapter()
  {
    PipelineAdapter = GetNode<PlayerPipeline>($"../../../Players/{PiecesManager.Name}");
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

  private void SetHexMap()
  {
    HexMap = GameManager.GetNode<HexMap>("HexMap");
  }

  public void Decorate(PieceStateDecorator stateDecorator, PieceInstanceDecorator instanceDecorator)
  {
    State = stateDecorator;
    Instance = instanceDecorator;
  }

  public static PieceAdapter GetPiece(GameManager manager, int faction, string pieceName)
  {
    return manager.GetNode<Node>("Pieces").GetChild<Node>(faction).GetNode<PieceAdapter>(pieceName);
  }
}
