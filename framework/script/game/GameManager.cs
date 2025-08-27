using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

public partial class GameManager : Node2D
{
	private Node _players;
	private Dictionary<string, Group<Vector2I, PieceAdapter>> _pieces = [];

  [Export]
	public string GameName { get; set; } = "";

	public override void _Ready()
	{
		_players ??= GetNode<Node>("Players");
		GameState.Instance.IsSolo = true;
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
			_pieces[piecesManager.Name] = piecesManager.Pieces;
		}
	}

	public string HashState()
	{
		Dictionary<string, List<Dictionary<string, object>>> state = [];
		foreach (var faction in _pieces)
		{
			state[faction.Key] = [];
			foreach (var pieces in faction.Value.Wrapped)
			{
				foreach (var piece in pieces.Value)
				{
					Dictionary<string, object> pieceState = [];
					pieceState["position"] = piece.State.As<IPositionable>().MapPosition;
					pieceState["name"] = piece.Name;
					pieceState["pieceType"] = piece.PieceType;
					pieceState["faction"] = piece.Faction;
					state[faction.Key].Add(pieceState);
				}
			}
		}
		var json = GithubUtils.Serialize(state);
		return GithubUtils.HashState(json);
	}
}
