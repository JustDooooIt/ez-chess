using System;
using System.Linq;
using Godot;

public partial class PieceInstance : Node2D, IPieceInstance
{
	public event Action<int> ActionCompleted;

	private Tween _tween;
	private Area2D _area;
	private Shader _outline;

	public Tween Tween => GetTween();
	public bool Selectable { get; set; } = true;
	private TileMapLayer HexMap => GetNode<TileMapLayer>("../../../../HexMap/BaseTerrain");
  public PipelineAdapter PipelineAdapter { get; set; }
  public PiecesManager PiecesManager { get; set; }
  public PieceAdapter PieceAdapter { get; set; }
  public IInterfaceQueryable Wrapper { get; set; }
  public IInterfaceQueryable Proxy => ((IPiece)this).GetProxy();
	public GodotObject Origin => this;

  public override void _Ready()
	{
		_area = GetNode<Area2D>("Area2D");
		_area.MouseEntered += Select;
		_area.MouseExited += CancelSelect;
		_outline ??= GD.Load<Shader>("res://shader/outline.gdshader");
	}

	private void Select()
	{
		SetOutline(true);
	}

	private void CancelSelect()
	{
		SetOutline(false);
	}

	private Tween GetTween()
	{
		if (_tween == null)
		{
			return CreateTween();
		}
		else
		{
			if (_tween.IsValid())
				return _tween;
			else
				return CreateTween();
		}
	}

	public void AddCover(Texture2D texture, int faceIndex, bool defaultFace = false)
	{
		_outline ??= GD.Load<Shader>("res://shader/outline.gdshader");
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
		(sprite.Material as ShaderMaterial).SetShaderParameter("enable_outline", opened);
	}

	public void SetAreaSize(Vector2 size)
	{
		var area = GetNode<Area2D>("Area2D");
		var shape = area.GetNode<CollisionShape2D>("CollisionShape2D");
		if (shape.Shape is RectangleShape2D rectangle)
		{
			rectangle.Size = size;
		}
	}

	public void SetPosition(Vector2I position)
	{
		Position = HexMap.MapToLocal(position) + HexMap.Position;
	}
}
