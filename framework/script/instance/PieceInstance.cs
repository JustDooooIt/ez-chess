using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;

public partial class PieceInstance : Node2D, IPieceInstance
{
	public event Action<int> ActionCompleted;

	public readonly Color EnemyColor = Colors.Red;
	public readonly Color FriendliesColor = Colors.Yellow;
	public readonly Color MyselfColor = Colors.Green;

	private Tween _tween;
	private Area2D _area;
	private Shader _outline;
	private TerrainLayers _terrainLayers;

	// public Tween Tween { get => GetTween(); set => _tween = value; }
	public bool IsSelected { get; set; } = false;
	public HexMap HexMap { get; set; }
	public TerrainLayers TerrainLayers { get => _terrainLayers; set => _terrainLayers = value; }
	public PlayerPipeline PipelineAdapter { get; set; }
	public PiecesManager PiecesManager { get; set; }
	public PieceAdapter PieceAdapter { get; set; }
	public IInterfaceQueryable Proxy => ((IPiece)this).GetProxy();
	public IInterfaceQueryable Wrapper { get; set; }
	public GodotObject Origin => this;
	public IPiece Wrapped => this;
	public Area2D Area { get => _area; set => _area = value; }
	public bool Selectable { get; set; } = true;
	public bool IsHover { get; set; } = false;
	public int Faction { get; set; }
	public int PieceType { get; set; }

	public override void _Ready()
	{
		_area = GetNode<Area2D>("Area2D");
		_area.MouseEntered += Hover;
		_area.MouseExited += UnHover;
		_area.InputEvent += OnInputEvent;
		_terrainLayers = GetNode<TerrainLayers>("../../../../HexMap/TerrainLayers");
		_outline ??= GD.Load<Shader>("res://framework/shader/outline.gdshader");
		HexMap = GetNode<HexMap>("../../../../HexMap");
	}

	private void Hover()
	{
		IsHover = true;
		if (!IsSelected)
		{
			SetOutline(true);
		}
	}

	private void UnHover()
	{
		IsHover = false;
		if (!IsSelected)
		{
			SetOutline(false);
		}
	}

	private void OnInputEvent(Node viewport, InputEvent @event, long shapeIndex)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonMask == MouseButtonMask.Left)
			{
				IsSelected = Selectable;
				if (Selectable)
					OnPieceLeftClicked(mouseButton);
			}
		}
	}

	private void OnPieceLeftClicked(InputEventMouseButton @event)
	{
		SetOutline(IsSelected);
		GameState.Instance.SelectedPiece = PieceAdapter;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (mouseButton.ButtonMask == MouseButtonMask.Left)
			{
				if (!IsHover)
				{
					IsSelected = false;
					SetOutline(false);
				}
			}
		}
	}


	// private Tween GetTween()
	// {
	// if (_tween == null)
	// {
	// 	_tween = CreateTween();
	// 	return _tween;
	// }
	// else
	// {
	// 	if (_tween.IsValid())
	// 		return _tween;
	// 	else
	// 	{
	// 		_tween = CreateTween();
	// 		return _tween;
	// 	}
	// }
	// }

	public void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false)
	{
		_outline ??= GD.Load<Shader>("res://framework/shader/outline.gdshader");
		var sprites = GetNode<Node2D>("Sprites");
		var topSprite = GetNode<Node2D>("TopSprite");
		var sprite = new Sprite2D { Texture = texture };
		var material = new ShaderMaterial();
		sprite.Material = material;
		material.Shader = _outline;
		sprite.Name = faceIndex.ToString();
		if (defaultFace)
		{
			topSprite.AddChild(sprite);
		}
		else
		{
			sprites.AddChild(sprite);
		}
	}

	public void SetOutline(bool opened)
	{
		var sprite = GetNode<Node2D>("TopSprite").GetChild<Sprite2D>(0);
		var material = (ShaderMaterial)sprite.Material;
		material.SetShaderParameter("enable_outline", opened);
		if (PieceAdapter.Faction == GameState.Instance.PlayerFaction)
			material.SetShaderParameter("outline_color", MyselfColor);
		else
			material.SetShaderParameter("outline_color", EnemyColor);
	}

	public void SetAreaSize(Godot.Vector2 size)
	{
		var area = GetNode<Area2D>("Area2D");
		var shape = area.GetNode<CollisionShape2D>("CollisionShape2D");
		if (shape.Shape is RectangleShape2D rectangle)
		{
			rectangle.Size = size;
		}
	}

	public Godot.Vector2 GetAreaSize()
	{
		var area = GetNode<Area2D>("Area2D");
		var shape = area.GetNode<CollisionShape2D>("CollisionShape2D");
		if (shape.Shape is RectangleShape2D rectangle)
		{
			return rectangle.Size;
		}
		return default;
	}

	public void SetPosition(Vector2I position)
	{
		Position = TerrainLayers.BaseTerrain.MapToLocal(position) + TerrainLayers.Position;
	}

	public IEnumerable<T> QueryAll<T>() where T : class
	{
		return [];
	}

}
