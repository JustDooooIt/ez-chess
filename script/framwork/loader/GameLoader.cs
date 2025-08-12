using System;
using System.Collections.Generic;
using System.Linq;
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

	[Export]
	public string ConfigPath { get; set; }
	[Export]
	public CSharpScript PieceFactoryScript { get; set; }

	public override void _Ready()
	{
		_manager = GetNode<GameManager>("..");
		_map = GetNode<HexMap>("../HexMap");
		_piecesContainer = GetNode<Node>("../Pieces");
		_config = LoadConfig();
		_pieceFactory = (IPieceFactory)PieceFactoryScript.New().AsGodotObject();
		InitFirstFounded(_config);
	}

	private IDictionary<string, Variant> LoadConfig()
	{
		var file = FileAccess.Open("res://example/resource/config.json", FileAccess.ModeFlags.Read);
		return Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();
	}

	private void InitFirstFounded(IDictionary<string, Variant> config)
	{
		var factionNames = config["factions"].AsGodotArray<string>();
		foreach (var factionName in factionNames)
		{
			AddPlayer(factionName);
			if (config.TryGetValue(factionName, out var vfaction))
			{
				var faction = vfaction.AsGodotDictionary<string, Variant>();
				var pieces = faction["pieces"].AsGodotArray();
				var factionNode = GetNodeOrNull<Node>($"../{factionName}");
				if (factionNode == null)
				{
					factionNode = new Node { Name = factionName };
					_piecesContainer.AddChild(factionNode);
				}
				foreach (var vpiece in pieces)
				{
					var piece = vpiece.AsGodotDictionary<string, Variant>();
					var pieceName = piece["name"].AsString();
					var faces = piece["faces"].AsGodotArray();
					var pieceType = piece["type"].AsInt16();
					var position = piece["position"].AsGodotDictionary<string, int>();
					var positionVec = new Vector2I(position["x"], position["y"]);
					var size = piece["size"].AsGodotDictionary<string, int>();
					var sizeVec = new Vector2I(size["x"], size["y"]);
					List<Texture2D> faceImage = [];
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
					var pieceAdapter = _pieceFactory.Create(pieceType, states);
					pieceAdapter.Name = pieceName;
					faceImage.ForEach(pieceAdapter.Instance.AddCover);
					pieceAdapter.Instance.SetAreaSize(sizeVec);
					factionNode.AddChild(pieceAdapter);
					_map.PlacePiece((Node2D)pieceAdapter.Instance.GetOrigin(), positionVec);
					var a = _manager.GetNode("Players");
					var statePipeline = _manager.GetNode<Pipeline>($"Players/{factionName}/State");
					var renderPipeline = _manager.GetNode<Pipeline>($"Players/{factionName}/Render"); ;
					pieceAdapter.StatePipeline = statePipeline;
					pieceAdapter.RenderPipeline = renderPipeline;
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
}
