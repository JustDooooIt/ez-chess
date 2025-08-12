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
	private IDictionary<string, Variant> _config;
	private GameManager _manager;
	private HexMap _map;
	private Node _piecesContainer;
	private IPieceFactory _pieceFactory;
	private PackedScene _piecesManagerScene;

	[Export]
	public string ConfigPath { get; set; }
	[Export]
	public CSharpScript PieceFactoryScript { get; set; }

	public override void _Ready()
	{
		_manager = GetNode<GameManager>("..");
		_map = GetNode<HexMap>("../HexMap");
		_piecesContainer = GetNode<Node>("../Pieces");
		_piecesManagerScene = GD.Load<PackedScene>("res://scene/pieces_manager.tscn");
		_config = LoadConfig();
		_pieceFactory = (IPieceFactory)PieceFactoryScript.New().AsGodotObject();
		InitFirstFounded(_config);
		StartPipeline();
	}

	private IDictionary<string, Variant> LoadConfig()
	{
		var file = FileAccess.Open("res://example/resource/config.json", FileAccess.ModeFlags.Read);
		return Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();
	}

	private async void InitFirstFounded(IDictionary<string, Variant> config)
	{
		var factionNames = config["factions"].AsGodotArray<string>();
		foreach (var factionName in factionNames)
		{
			AddPlayer(factionName);
			var factionNode = CreateFactionNode(factionName);
			if (config.TryGetValue(factionName, out var vfaction))
			{
				var faction = vfaction.AsGodotDictionary<string, Variant>();
				var pieces = faction["pieces"].AsGodotArray();
				foreach (var piecev in pieces)
				{
					var piece = piecev.AsGodotDictionary<string, Variant>();
					var pieceName = piece["name"].AsString();
					var faces = piece["faces"].AsGodotArray();
					var pieceType = piece["type"].AsInt16();
					var position = piece["position"].AsGodotDictionary<string, int>();
					var positionVec = new Vector2I(position["x"], position["y"]);
					var size = piece["size"].AsGodotDictionary<string, int>();
					var sizeVec = new Vector2I(size["x"], size["y"]);
					Array<Texture2D> faceImage = [];
					Array<Godot.Collections.Dictionary<string, Variant>> states = [];
					foreach (var facev in faces)
					{
						var face = facev.AsGodotDictionary<string, Variant>();
						var imagePath = face["image"].AsString();
						var image = GD.Load<Texture2D>(imagePath);
						var property = face["property"].AsGodotDictionary<string, Variant>();
						faceImage.Add(image);
						states.Add(property);
					}
					int defaultFace = faces.Select(e => e.AsGodotDictionary<string, Variant>()).ToList().FindIndex(e => e.ContainsKey("default") && e["default"].AsBool());
					defaultFace = defaultFace == -1 ? 0 : defaultFace;
					var pieceAdapter = _pieceFactory.Create(pieceType, pieceName, faceImage, defaultFace, sizeVec, states);
					factionNode.AddChild(pieceAdapter);
					if (!_manager.IsNodeReady())
						await ToSignal(_manager, "ready");
					_map.PlacePiece(pieceAdapter, positionVec);
				}
			}
		}
	}

	private void AddPlayer(string name)
	{
		var pipelineScene = GD.Load<PackedScene>("res://scene/player.tscn");
		var pipeline = pipelineScene.Instantiate<PipelineAdapter>();
		pipeline.Name = name;
		_manager.AddPlayer(pipeline);
	}

	private Node CreateFactionNode(string name)
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
