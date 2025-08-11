using System;
using Godot;

public partial class PieceInstance : Node2D, IPieceInstance
{
	public event Action<int> ActionCompleted;

	private Tween _tween;
	private Area2D _area;

	public Tween Tween => GetTween();

  public Pipeline StatePipeline { get; set; }
  public Pipeline RenderPipeline { get; set; }

  public override void _Ready()
	{
		_area = GetNode<Area2D>("Area2D");
	}

	public GodotObject GetWrapped()
	{
		return this;
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

	public void AddCover(Texture2D texture)
	{
		var sprite = new Sprite2D { Texture = texture };
		AddChild(sprite, false, InternalMode.Front);
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
}
