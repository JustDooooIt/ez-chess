using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

public partial class GameManager : Node2D
{
	protected Node _players;
	private Button _endStage;
	public List<Group<Vector2I, PieceAdapter>> Pieces { get; } = [];

	[Export]
	public string GameName { get; set; } = "";
	public override void _Ready()
	{
		_players ??= GetNode<Node>("Players");
		GameState.Instance.IsSolo = true;
		_endStage = GetNode<Button>("CanvasLayer/Control/EndStage");
		_endStage.Pressed += () =>
		{

		};
	}

	public void AddPlayer(PlayerPipeline pipeline)
	{
		_players ??= GetNode<Node>("Players");
		_players.AddChild(pipeline);
	}

	public List<PlayerPipeline> GetPipelines()
	{
		_players ??= GetNode<Node>("Players");
		return [.. _players.GetChildren().Cast<PlayerPipeline>()];
	}

	public void StartPipelines()
	{
		foreach (var adapter in GetPipelines())
		{
			adapter.StatePipeline.Launch();
			adapter.RenderPipeline.Launch();
		}
	}

	public void InitPieces()
	{
		var piecesRootNode = GetNode<Node>("Pieces");
		var pieces = piecesRootNode.GetChildren().Cast<PiecesManager>().ToList();
		foreach (var piecesManager in pieces)
		{
			Pieces.Add(piecesManager.Pieces);
		}
	}

	public PieceAdapter GetPiece(int faction, string name)
	{
		return GetNode<Node>("Pieces").GetChild(faction).GetNode<PieceAdapter>(name);
	}

	// public string HashState()
	// {
	// 	Dictionary<string, List<Dictionary<string, object>>> state = [];
	// 	foreach (var faction in _pieces)
	// 	{
	// 		state[faction.Key] = [];
	// 		foreach (var pieces in faction.Value.Wrapped)
	// 		{
	// 			foreach (var piece in pieces.Value)
	// 			{
	// 				Dictionary<string, object> pieceState = [];
	// 				pieceState["position"] = pieces.Key;
	// 				pieceState["name"] = piece.Name;
	// 				pieceState["pieceType"] = piece.PieceType;
	// 				pieceState["faction"] = piece.Faction;
	// 				state[faction.Key].Add(pieceState);
	// 			}
	// 		}
	// 	}
	// 	var json = GithubUtils.Serialize(state);
	// 	return GithubUtils.HashState(json);
	// }
}
