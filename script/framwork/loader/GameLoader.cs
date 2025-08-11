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
    SetFirstFounded(config);
  }

  private IDictionary<string, Variant> LoadConfig()
  {
    var file = FileAccess.Open("res://example/resource/config.json", FileAccess.ModeFlags.Read);
    return Json.ParseString(file.GetAsText()).AsGodotDictionary<string, Variant>();
  }

  private void SetFirstFounded(IDictionary<string, Variant> config)
  {
    var factionNames = config["factions"].AsGodotArray<string>();
    foreach (var name in factionNames)
    {
      if (config.TryGetValue(name, out var vfaction))
      {
        var faction = vfaction.AsGodotDictionary<string, Variant>();
        var pieces = faction["pieces"].AsGodotArray();
        var baseScenePath = config["basePiece"].AsString();
        var baseScene = GD.Load<PackedScene>(baseScenePath);
        foreach (var vpiece in pieces)
        {
          var piece = vpiece.AsGodotDictionary<string, Variant>();
          var pieceName = piece["name"].AsString();
          var imagePath = piece["imagePath"].AsGodotArray<string>();
          var pieceScenePath = piece["scene"].AsString();
          var pieceScene = GD.Load<PackedScene>(pieceScenePath);
          var instance = pieceScene.Instantiate<PieceInstance>();
          var basePiece = baseScene.Instantiate<PieceAdapter>();
          var position = piece["position"].AsGodotDictionary<string, Variant>();
          var positionVec = new Vector2I(position["x"].AsInt32(), position["y"].AsInt32());
          foreach (var path in imagePath)
          {
            var image = GD.Load<Texture2D>(path);
            instance.AddCover(image);
          }
          map.PlacePiece(instance, positionVec);
          basePiece.Name = pieceName;
          basePiece.AddChild(instance);
          piecesContainer.AddChild(basePiece);
        }
      }
    }
  }

  public void InitPiece()
  {

  }
}
