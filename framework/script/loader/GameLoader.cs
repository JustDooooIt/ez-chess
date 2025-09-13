using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

/// <summary>
/// 设置棋盘初设
/// </summary>
public partial class GameLoader : Node
{
	public event Action GameLoaded;

	private Dictionary _config;
	private GameManager _manager;
	private HexMap _map;
	private Node _piecesContainer;
	private IPieceFactory _pieceFactory;
	private PackedScene _piecesManagerScene;
	private BaseRenderEventHandler _renderEventHandler;
	private IOperationRunner _operationRunner;
	private IEnvironmentRunner _environmentRunner;
	private Pipelines _pipelines;

	[Export]
	public string ConfigPath { get; set; }
	[Export]
	public CSharpScript PieceFactoryScript { get; set; }
	[Export]
	public CSharpScript RenderEventHandlerScript { get; set; }
	[Export]
	public CSharpScript OperationRunner { get; set; }
	[Export]
	public CSharpScript EnvironmentRunner { get; set; }
	[Export]
	public int Stages { get; set; }

	public override void _Ready()
	{
		_manager = GetNode<GameManager>("..");
		_map = GetNode<HexMap>("../HexMap");
		_piecesContainer = GetNode<Node>("../Pieces");
		_piecesManagerScene = GD.Load<PackedScene>("res://framework/scene/pieces_manager.tscn");
		_config = LoadConfig();
		_pieceFactory = (IPieceFactory)PieceFactoryScript.New().AsGodotObject();
		_renderEventHandler = (BaseRenderEventHandler)RenderEventHandlerScript.New().AsGodotObject();
		_pipelines = GetNode<Pipelines>("/root/Game/Players");
		_operationRunner = OperationRunner.New().AsGodotObject() as IOperationRunner;
		_environmentRunner = EnvironmentRunner.New().AsGodotObject() as IEnvironmentRunner;
		_pipelines.OperationRunner = _operationRunner;
		_pipelines.EnvironmentRunner = _environmentRunner;
		GameState.Instance.StageCount = Stages;
		InitFirstFounded(_config);
		StartPipeline();
	}

	private Dictionary LoadConfig()
	{
		// var file = FileAccess.Open(ConfigPath, FileAccess.ModeFlags.Read);
		// return Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();
		return GameState.Instance.Config;
	}

	private async void InitFirstFounded(Dictionary config)
	{
		var factionNames = config["factions"].AsGodotArray<string>();
		GameState.Instance.Factions = [.. factionNames];
		GameState.Instance.CurOperatorFaction = factionNames.ToList().FindIndex(e => e == config["firstMover"].AsString());
		int factionId = 0;
		IDictionary<string, int> defaultSize;
		Vector2I defaultSizeVec = Vector2I.Zero;
		if (config.TryGetValue("defaultSize", out Variant value))
		{
			defaultSize = value.AsGodotDictionary<string, int>();
			defaultSizeVec = new Vector2I(defaultSize["x"], defaultSize["y"]);
		}
		GameState.Instance.RoomState = new()
		{
			GameName = config["name"].AsString(),
			Seats = [.. factionNames.Select(e => "")],
		};
		foreach (var factionName in factionNames)
		{
			AddPlayer(factionName);
			var factionNode = CreateFactionNode(factionName);
			if (config.TryGetValue(factionName, out var vfaction))
			{
				var faction = vfaction.AsGodotDictionary<string, Variant>();
				var group = faction["group"].AsInt32();
				var pieces = faction["pieces"].AsGodotArray();
				foreach (var piecev in pieces)
				{
					var piece = piecev.AsGodotDictionary<string, Variant>();
					var pieceName = piece["name"].AsString();
					var faces = piece["faces"].AsGodotArray();
					var pieceType = piece["type"].AsInt16();
					var position = piece["position"].AsGodotDictionary<string, int>();
					var positionVec = new Vector2I(position["x"], position["y"]);
					IDictionary<string, int> size;
					Vector2I sizeVec;
					if (piece.TryGetValue("size", out var sizeVar))
					{
						size = sizeVar.AsGodotDictionary<string, int>();
						sizeVec = new Vector2I(size["x"], size["y"]);
					}
					else
					{
						sizeVec = defaultSizeVec;
					}
					Array<Texture2D> faceImage = [];
					Array<Godot.Collections.Dictionary<string, Variant>> property = [];
					foreach (var facev in faces)
					{
						var face = facev.AsGodotDictionary<string, Variant>();
						var imagePath = face["image"].AsString();
						var image = GD.Load<Texture2D>(imagePath);
						var propertyv = face["property"].AsGodotDictionary<string, Variant>();
						faceImage.Add(image);
						property.Add(propertyv);
					}
					int defaultFace = faces.Select(e => e.AsGodotDictionary<string, Variant>()).ToList().FindIndex(e => e.ContainsKey("default") && e["default"].AsBool());
					defaultFace = defaultFace == -1 ? 0 : defaultFace;
					var pieceAdapter = _pieceFactory.Create(pieceType, group, factionId, pieceName, faceImage, defaultFace, sizeVec, property);
					pieceAdapter.GameManager = _manager;
					factionNode.AddChild(pieceAdapter);
					factionNode.AddPiece(positionVec, pieceAdapter);
					GameLoaded += () => pieceAdapter.State.Query<IPositionEventSender>().SendPositionEvent(positionVec);
				}
			}
			factionId++;
		}
		if (!_manager.IsNodeReady())
			await ToSignal(_manager, "ready");
		_manager.InitPieces();
		GameLoaded?.Invoke();
	}

	private async void AddPlayer(string name)
	{
		PackedScene pipelineScene;
		if (GameState.Instance.PlayerFactionName == name)
		{
			pipelineScene = GD.Load<PackedScene>("res://framework/scene/player.tscn");
		}
		else
		{
			pipelineScene = GD.Load<PackedScene>("res://framework/scene/other.tscn");
		}
		var pipeline = pipelineScene.Instantiate<PlayerPipeline>();
		pipeline.Name = name;
		_manager.AddPlayer(pipeline);
		GameState.Instance.PlayerCount++;
		if (!pipeline.IsNodeReady())
			await ToSignal(pipeline, "ready");
		_renderEventHandler.Pipeline = pipeline.RenderPipeline;
		_renderEventHandler.GameManager = _manager;
		pipeline.RenderPipeline.RenderEventHandler = _renderEventHandler;
	}

	private PiecesManager CreateFactionNode(string name)
	{
		var factionNode = _piecesManagerScene.Instantiate<PiecesManager>();
		factionNode.Name = name;
		_piecesContainer.AddChild(factionNode);
		return factionNode;
	}

	private async void StartPipeline()
	{
		if (!_manager.IsNodeReady())
			await ToSignal(_manager, "ready");
		_manager.StartPipelines();
	}

}
