using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class GameLoader : Node
{
	private IDictionary<string, Variant> config;
	private GameManager manager;
	private HexMap map;
	private Node piecesContainer;

	[Export]
	public string ConfigPath { get; set; }

	public override void _Ready()
	{
		manager = GetNode<GameManager>("..");
		map = GetNode<HexMap>("../HexMap");
		piecesContainer = GetNode<Node>("../Pieces");
		config = LoadConfig();
		InitFirstFounded(config);
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
			if (config.TryGetValue(factionName, out var vfaction))
			{
				var faction = vfaction.AsGodotDictionary<string, Variant>();
				var pieces = faction["pieces"].AsGodotArray();
				var baseScenePath = config["basePiece"].AsString();
				var baseScene = GD.Load<PackedScene>(baseScenePath);
				var factionNode = GetNodeOrNull<Node>($"../{factionName}");
				if (factionNode == null)
				{
					factionNode = new Node { Name = factionName };
					piecesContainer.AddChild(factionNode);
				}
				foreach (var vpiece in pieces)
				{
					var piece = vpiece.AsGodotDictionary<string, Variant>();
					var pieceName = piece["name"].AsString();
					var imagePath = piece["imagePath"].AsGodotArray<string>();
					var pieceScenePath = piece["scene"].AsString();
					var pieceScene = GD.Load<PackedScene>(pieceScenePath);
					var instance = pieceScene.Instantiate<PieceInstance>();
					var basePiece = baseScene.Instantiate<PieceAdapter>();
					var position = piece["position"].AsGodotDictionary<string, int>();
					var positionVec = new Vector2I(position["x"], position["y"]);
					var size = piece["size"].AsGodotDictionary<string, int>();
					var sizeVec = new Vector2I(size["x"], size["y"]);
					foreach (var path in imagePath)
					{
						var image = GD.Load<Texture2D>(path);
						instance.AddCover(image);
					}
					instance.SetAreaSize(sizeVec);
					basePiece.Name = pieceName;
					basePiece.AddChild(instance);
					factionNode.AddChild(basePiece);
					map.PlacePiece(instance, positionVec);
					AddPlayer(factionName);
					instance.StatePipeline = manager.GetNode<Pipeline>($"Players/{factionName}/State");
					instance.RenderPipeline = manager.GetNode<Pipeline>($"Players/{factionName}/Render");
					basePiece.StatePipeline = manager.GetNode<Pipeline>($"Players/{factionName}/State");
					basePiece.RenderPipeline = manager.GetNode<Pipeline>($"Players/{factionName}/Render");
				}
			}
		}
	}

	private void AddPlayer(string name)
	{
		var pipelineScene = GD.Load<PackedScene>("res://scene/player.tscn");
		var pipeline = pipelineScene.Instantiate<PipelineAdapter>();
		pipeline.Name = name;
		manager.AddPlayer(pipeline);
	}
}
